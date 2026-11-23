"""Prueba HTTPS exclusivamente contra el servidor local de integración."""
import json
import secrets
import ssl
import urllib.error
import urllib.request
import uuid

BASE = "https://localhost:7239/api/"
# Solo el servidor de prueba en loopback usa el certificado de desarrollo.
TLS = ssl._create_unverified_context()

def request(method, path, status, body=None, token=None):
    headers = {"Content-Type": "application/json"}
    if token:
        headers["Authorization"] = "Bearer " + token
    data = json.dumps(body).encode() if body is not None else None
    req = urllib.request.Request(BASE + path, data=data, headers=headers, method=method)
    try:
        response = urllib.request.urlopen(req, context=TLS, timeout=10)
    except urllib.error.HTTPError as error:
        response = error
    with response:
        assert response.status == status, (method, path, status, response.status)
        raw = response.read()
    return json.loads(raw) if raw else None

def account():
    credentials = {"correo": uuid.uuid4().hex + "@example.invalid", "password": secrets.token_urlsafe(24)}
    registration = {**credentials, "nombre": "Integración", "rol": "Administrador"}
    profile = request("POST", "auth/register", 201, registration)
    assert profile["rol"] == "Usuario" and "passwordHash" not in profile
    request("POST", "auth/register", 409, registration)
    request("POST", "auth/login", 401, {**credentials, "password": "incorrecta"})
    token = request("POST", "auth/login", 200, credentials)["token"]
    assert request("GET", "auth/me", 200, token=token) == profile
    request("GET", "auth/me", 401, token=token+"x")
    return token

def run():
    request("GET", "auth/me", 401)
    a, b = account(), account()
    cat = {"nombre": "Comida", "tipo": "Gasto"}
    cid = request("POST", "categorias", 201, cat, a)["idCategoria"]
    category = f"categorias/{cid}"
    request("PUT", category, 200, {**cat, "nombre":"Alimentos"}, a)
    assert request("GET", "categorias", 200, token=b) == []
    request("DELETE", category, 404, token=b)
    mov = {"idCategoria":cid, "tipo":"Gasto", "fecha":"2026-11-01", "cantidad":24.75}
    request("POST", "movimientos", 404, mov, b)
    request("POST", "movimientos", 400, {**mov, "cantidad":0.001}, a)
    mid = request("POST", "movimientos", 201, mov, a)["idMovimiento"]
    movement = f"movimientos/{mid}"
    request("DELETE", category, 409, token=a)
    assert request("GET", movement, 200, token=a)["cantidad"] == 24.75
    for method in ("GET","PUT","DELETE"):
        request(method, movement, 404, mov if method=="PUT" else None, b)
    request("PUT", movement, 200, {**mov,"cantidad":30}, a)
    page = request("GET", f"movimientos?idCategoria={cid}&tipo=Gasto&desde=2026-11-01&hasta=2026-11-30&tamano=1", 200, token=a)
    assert page["total"] == 1 and page["items"][0]["cantidad"] == 30
    assert request("GET","movimientos",200,token=b)["total"] == 0
    period = "?mes=11&anio=2026"
    budget = {"idCategoria":cid, "mes":11, "anio":2026, "limite":100}
    bid = request("POST","presupuestos",201,budget,a)["idPresupuesto"]
    request("POST","presupuestos",409,budget,a)
    request("PUT",f"presupuestos/{bid}",404,budget | {"idCategoria":cid},b)
    request("PUT",f"presupuestos/{bid}",200,budget | {"limite":90},a)
    assert request("GET","presupuestos"+period,200,token=a)[0]["gastado"] == 30
    assert request("GET","presupuestos"+period,200,token=b) == []
    summary = request("GET","dashboard/resumen"+period,200,token=a)
    assert (summary["saldo"],summary["ingresos"],summary["gastos"]) == (-30,0,30)
    assert request("GET","dashboard/estadisticas"+period,200,token=a)[0]["total"] == 30
    request("DELETE",f"presupuestos/{bid}",404,token=b)
    request("DELETE",f"presupuestos/{bid}",204,token=a)
    target = {"mes":12,"anio":2026}
    next_month = "?mes=12&anio=2026"
    prediction = request("GET","asistente/prediccion"+next_month,200,token=a)
    assert prediction["categorias"][0]["estimacion"]["monto"] == 30
    assert request("GET","asistente/prediccion"+period,200,token=a)["categorias"][0]["estimacion"] is None
    assert request("GET","asistente/prediccion"+next_month,200,token=b)["categorias"] == []
    suggestions = request("POST","presupuestos/sugerencia",200,target,a)
    assert suggestions["requiereConfirmacion"] and suggestions["categorias"][0]["estimacion"]["sugerido"] == 31.5
    answer = request("POST","asistente/chat",200,{"mensaje":"gastos", "mes":11,"anio":2026},a)
    assert "30.00" in answer["respuesta"]
    assert len(request("GET","asistente/chat",200,token=a))==2
    assert request("GET","asistente/chat",200,token=b)==[]
    assert request("GET","presupuestos"+next_month,200,token=a)==[]
    confirm = {**target,"idCategoria":cid,"montoConfirmado":31.5,"confirmado":False}
    route = "presupuestos/sugerencia/confirmar"
    request("POST",route,400,confirm,a)
    request("POST",route,409,{**confirm,"confirmado":True,"montoConfirmado":29},a)
    applied=request("POST",route,201,{**confirm,"confirmado":True},a)
    assert applied["origen"]=="IA" and applied["limite"]==31.5
    request("POST",route,409,{**confirm,"confirmado":True},a)
    request("DELETE",f"presupuestos/{applied['idPresupuesto']}",204,token=a)
    request("DELETE",movement,204,token=a)
    request("GET","asistente/prediccion"+next_month,200,token=a)
    request("DELETE",category,204,token=a)
    print("OK: sesión, categorías, movimientos, filtros y aislamiento de dos cuentas.")

if __name__ == "__main__":
    run()

