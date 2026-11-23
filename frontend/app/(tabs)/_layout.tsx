import { Tabs } from 'expo-router';
export default function TabLayout() {
  return <Tabs screenOptions={{tabBarActiveTintColor:'#751d39',headerShown:true}}>
    <Tabs.Screen name="index" options={{title:'Resumen'}}/><Tabs.Screen name="movimientos" options={{title:"Movimientos"}}/><Tabs.Screen name="two" options={{title:'Perfil'}}/>
  </Tabs>;
}