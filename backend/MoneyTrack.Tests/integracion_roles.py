"""Ejecutar únicamente contra la API conectada a MoneyTrackAuthTests."""
import os
import secrets
import subprocess
import uuid
from integracion_https import request

def registro():
    cred = {"correo":uuid.uuid4().hex+"@example.invalid","password":secrets.token_urlsafe(24)}
    perfil = request("POST","auth/register",201,{**cred,"nombre":"Prueba de permisos"})
    return cred,perfil["idUsuario"]

def login(cred):
    return request("POST","auth/login",200,cred)["token"]

admin, admin_id = registro()
user, user_id = registro()
anterior = login(admin)
request("GET","admin/usuarios",403,token=anterior)
bootstrap = """
$ErrorActionPreference='Stop'
$c=New-Object System.Data.SqlClient.SqlConnection 'Server=(localdb)\\MoneyTrack;Database=MoneyTrackAuthTests;Integrated Security=True'
try {
    $c.Open()
    $q=$c.CreateCommand()
    $q.CommandText="UPDATE Usuarios SET rol='Administrador' WHERE id_usuario=@id AND correo=@correo"
    [void]$q.Parameters.AddWithValue('@id',[int]$env:MT_TEST_ID)
    [void]$q.Parameters.AddWithValue('@correo',$env:MT_TEST_CORREO)
    if($q.ExecuteNonQuery() -ne 1){throw 'Cuenta de prueba ausente'}
} finally {$c.Dispose()}
"""
subprocess.run(["powershell","-NoProfile","-Command",bootstrap],check=True,
               env={**os.environ,"MT_TEST_ID":str(admin_id),"MT_TEST_CORREO":admin["correo"]})
request("GET","auth/me",401,token=anterior)
token=login(admin)
request("GET","admin/usuarios",200,token=token)
request("PUT",f"admin/usuarios/{admin_id}/rol",409,{"rol":"Usuario"},token)
request("PUT",f"admin/usuarios/{user_id}/rol",204,{"rol":"Administrador"},token)
promovido=login(user)
request("GET","admin/usuarios",200,token=promovido)
request("PUT",f"admin/usuarios/{user_id}/rol",204,{"rol":"Usuario"},token)
request("GET","admin/usuarios",401,token=promovido)
request("GET","admin/usuarios",403,token=login(user))
print("OK: roles, autorización y rechazo de tokens con rol obsoleto.")
