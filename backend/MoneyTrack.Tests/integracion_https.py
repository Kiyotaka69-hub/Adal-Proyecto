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
    request("DELETE",movement,204,token=a)
    request("DELETE",category,204,token=a)
    print("OK: sesión, categorías, movimientos, filtros y aislamiento de dos cuentas.")

if __name__ == "__main__":
    run()

