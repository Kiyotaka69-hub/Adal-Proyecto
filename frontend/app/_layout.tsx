import { Stack } from 'expo-router';
import { useEffect } from 'react';
import { ActivityIndicator, Text } from 'react-native';
import { restaurar, useSesion } from '@/lib/sesion';
import { Accion, Pantalla } from '@/components/Pantalla';
export default function RootLayout() {
  const {perfil,cargando,error}=useSesion();
  useEffect(()=>{void restaurar();},[]);
  if(cargando) return <ActivityIndicator accessibilityLabel="Comprobando sesión"/>;
  if(error) return <Pantalla titulo="Conexión"><Text>{error}</Text><Accion titulo="Reintentar" onPress={restaurar}/></Pantalla>;
  return <Stack screenOptions={{headerShown:false}}>
    <Stack.Protected guard={!perfil}><Stack.Screen name="acceso"/></Stack.Protected>
    <Stack.Protected guard={!!perfil}><Stack.Screen name="(tabs)"/></Stack.Protected>
  </Stack>;
}