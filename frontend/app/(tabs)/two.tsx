import { useState } from 'react';
import { Text } from 'react-native';
import { Accion, Pantalla } from '@/components/Pantalla';
import { mensaje,salir,useSesion } from '@/lib/sesion';
export default function Perfil(){
  const {perfil}=useSesion();const [error,setError]=useState('');
  return <Pantalla titulo={perfil?.nombre ?? 'Perfil'}><Text>{perfil?.correo}</Text><Text>{perfil?.rol}</Text>
    {!!error && <Text accessibilityRole="alert">{error}</Text>}
    <Accion titulo="Cerrar sesión" onPress={()=>{salir().catch(e=>setError(mensaje(e)));}}/></Pantalla>;
}