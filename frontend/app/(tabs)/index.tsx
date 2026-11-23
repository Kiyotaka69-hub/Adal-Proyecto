import { useFocusEffect } from 'expo-router';
import { useCallback,useState } from 'react';
import { ActivityIndicator,Text } from 'react-native';
import { Accion,Pantalla } from '@/components/Pantalla';
import { api,mensaje } from '@/lib/sesion';
type Resumen={saldo:number;ingresos:number;gastos:number};
const mxn=new Intl.NumberFormat('es-MX',{style:'currency',currency:'MXN'});
export default function Dashboard(){
  const [datos,setDatos]=useState<Resumen|null>(null),[error,setError]=useState('');
  const cargar=useCallback(()=>{
    const ctrl=new AbortController();
    const hoy=new Date();
    setDatos(null);setError('');
    api.get<Resumen>('dashboard/resumen',{signal:ctrl.signal,params:{mes:hoy.getMonth()+1,anio:hoy.getFullYear()}})
      .then(r=>{if(!ctrl.signal.aborted)setDatos(r.data);}).catch(e=>{if(!ctrl.signal.aborted)setError(mensaje(e));});
    return ()=>ctrl.abort();
  },[]);
  useFocusEffect(cargar);
  return <Pantalla titulo="Saldo disponible">{error?<><Text accessibilityRole="alert">{error}</Text><Accion titulo="Reintentar" onPress={cargar}/></>:
    datos?<><Text style={{fontSize:36}}>{mxn.format(datos.saldo)}</Text><Text>Ingresos del mes · {mxn.format(datos.ingresos)}</Text><Text>Gastos del mes · {mxn.format(datos.gastos)}</Text></>:<ActivityIndicator/>}</Pantalla>;
}