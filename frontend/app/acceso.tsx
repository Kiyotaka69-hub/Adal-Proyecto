import { useState } from 'react';
import { Text } from 'react-native';
import { Accion, Campo, Pantalla } from '@/components/Pantalla';
import { api, entrar, mensaje } from '@/lib/sesion';

export default function Acceso() {
  const [modo,setModo]=useState<'bienvenida'|'login'|'registro'>('bienvenida');
  const [nombre,setNombre]=useState(''),[correo,setCorreo]=useState(''),[password,setPassword]=useState('');
  const [error,setError]=useState(''),[ocupado,setOcupado]=useState(false);
  function abrir(paso:typeof modo){setModo(paso);setError('');setPassword('');}
  async function enviar(){
    if(!correo.trim().includes('@') || !password || (modo==='registro' && (!nombre.trim() || password.length<8))){
      setError('Revisa el correo y los campos. Para registrarte usa al menos ocho caracteres de contraseña.');return;
    }
    setOcupado(true);setError('');
    try{
      if(modo==='registro') await api.post('auth/register',{nombre:nombre.trim(),correo:correo.trim(),password});
      await entrar(correo.trim(),password);
    }catch(e){setError(mensaje(e));}finally{setOcupado(false);}
  }
  if(modo==='bienvenida')return <Pantalla titulo="MoneyTrack"><Text>Consulta tu saldo, registra tus gastos y organiza tu presupuesto mensual.</Text>
    <Accion titulo="Iniciar sesión" onPress={()=>abrir('login')}/><Accion titulo="Crear cuenta" onPress={()=>abrir('registro')}/></Pantalla>;
  return <Pantalla titulo={modo==='registro'?'Crear cuenta':'Iniciar sesión'}>
    {modo==='registro' && <Campo label="Nombre" value={nombre} onChangeText={setNombre} maxLength={120}/>}
    <Campo label="Correo" value={correo} onChangeText={setCorreo} autoCapitalize="none" keyboardType="email-address"/>
    <Campo label="Contraseña" value={password} onChangeText={setPassword} secureTextEntry autoCapitalize="none"/>
    {!!error && <Text accessibilityRole="alert">{error}</Text>}
    <Accion titulo={ocupado?'Enviando…':'Continuar'} disabled={ocupado} onPress={enviar}/>
    <Accion titulo="Volver" disabled={ocupado} onPress={()=>abrir('bienvenida')}/></Pantalla>;
}