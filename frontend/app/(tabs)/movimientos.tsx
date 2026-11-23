import {useCallback,useState} from 'react';
import {useFocusEffect} from 'expo-router';
import {Alert,Text,View} from 'react-native';
import {Accion,Campo,Pantalla} from '@/components/Pantalla';
import {api,mensaje} from '@/lib/sesion';

type Tipo='Ingreso'|'Gasto';
type Categoria={idCategoria:number;nombre:string;tipo:Tipo};
type Movimiento={idMovimiento:number;idCategoria:number;cantidad:number;tipo:Tipo;descripcion:string|null;fecha:string;esRecurrente:boolean};
const mxn=new Intl.NumberFormat('es-MX',{style:'currency',currency:'MXN'});
const fechaHoy=()=>new Intl.DateTimeFormat('sv-SE').format(new Date());
export default function Movimientos(){
  const [cats,setCats]=useState<Categoria[]>([]),[items,setItems]=useState<Movimiento[]>([]);
  const [pagina,setPagina]=useState(1),[total,setTotal]=useState(0),[error,setError]=useState(''),[ocupado,setOcupado]=useState(false);
  const [tipo,setTipo]=useState<Tipo>('Gasto'),[categoria,setCategoria]=useState(0),[importe,setImporte]=useState('');
  const [fecha,setFecha]=useState(fechaHoy),[descripcion,setDescripcion]=useState(''),[recurrente,setRecurrente]=useState(false);
  const [edicion,setEdicion]=useState<number|null>(null),[form,setForm]=useState(false),[nombre,setNombre]=useState('');
  const cargar=useCallback(async()=>{
    const [c,m]=await Promise.all([api.get<Categoria[]>('categorias'),api.get<{items:Movimiento[];total:number}>('movimientos',{params:{pagina,tamano:20}})]);
    setCats(c.data);setItems(m.data.items);setTotal(m.data.total);
  },[pagina]);
  useFocusEffect(useCallback(()=>{cargar().catch(e=>setError(mensaje(e)));},[cargar]));
  async function ejecutar(fn:()=>Promise<void>){
    setOcupado(true);setError('');
    try{await fn();await cargar();}catch(e){setError(mensaje(e));}finally{setOcupado(false);}
  }
  function abrir(t:Tipo,m?:Movimiento){
    setTipo(t);setCategoria(m?.idCategoria??0);setImporte(m?String(m.cantidad):'');setFecha(m?.fecha??fechaHoy());
    setDescripcion(m?.descripcion??'');setRecurrente(m?.esRecurrente??false);setEdicion(m?.idMovimiento??null);setForm(true);setError('');
  }
  async function guardar(){
    const valor=importe.trim().replace(',','.');
    if(!categoria || !/^\d{1,10}(\.\d{1,2})?$/.test(valor) || Number(valor)<=0 || !/^\d{4}-\d{2}-\d{2}$/.test(fecha)){
      setError('Selecciona categoría, importe positivo con hasta dos decimales y fecha AAAA-MM-DD.');return;
    }
    await ejecutar(async()=>{
      const body={tipo,idCategoria:categoria,cantidad:Number(valor),fecha,descripcion,esRecurrente:recurrente};
      if(edicion)await api.put('movimientos/'+edicion,body);else await api.post('movimientos',body);
      setForm(false);
    });
  }
  async function nuevaCategoria(){
    if(!nombre.trim()){setError('Escribe el nombre de la categoría.');return;}
    await ejecutar(async()=>{
      const r=await api.post<Categoria>('categorias',{tipo,nombre:nombre.trim()});
      setCategoria(r.data.idCategoria);setNombre('');
    });
  }
  return <Pantalla titulo={form?(edicion?'Editar movimiento':'Registrar '+tipo.toLowerCase()):'Movimientos'}>
    {!!error&&<Text accessibilityRole="alert">{error}</Text>}
    {form?<><Text>Categoría de {tipo.toLowerCase()}</Text>{cats.filter(c=>c.tipo===tipo).map(c=><Accion key={c.idCategoria} titulo={(c.idCategoria===categoria?'✓ ':'')+c.nombre} onPress={()=>setCategoria(c.idCategoria)}/>)}
      <Campo label="Nueva categoría" value={nombre} maxLength={100} onChangeText={setNombre}/><Accion titulo="Agregar categoría" disabled={ocupado} onPress={nuevaCategoria}/>
      <Campo label="Importe en MXN" keyboardType="decimal-pad" value={importe} onChangeText={setImporte}/>
      <Campo label="Fecha (AAAA-MM-DD)" value={fecha} onChangeText={setFecha}/>
      <Campo label="Descripción" value={descripcion} onChangeText={setDescripcion} maxLength={500}/>
      <Accion titulo={recurrente?'Recurrente: sí':'Recurrente: no'} onPress={()=>setRecurrente(!recurrente)}/>
      <Accion titulo="Guardar movimiento" disabled={ocupado} onPress={guardar}/><Accion titulo="Cancelar" disabled={ocupado} onPress={()=>setForm(false)}/>
    </>:<><Accion titulo="Registrar ingreso" onPress={()=>abrir('Ingreso')}/><Accion titulo="Registrar gasto" onPress={()=>abrir('Gasto')}/>
      {!items.length&&!error&&<Text>Sin movimientos en esta página.</Text>}
      {items.map(m=><View key={m.idMovimiento} style={{gap:8,borderBottomWidth:1,paddingVertical:12,borderColor:'#ded8db'}}>
        <Text>{m.fecha} · {cats.find(c=>c.idCategoria===m.idCategoria)?.nombre}</Text><Text>{m.tipo} · {mxn.format(m.cantidad)}</Text><Text>{m.descripcion}</Text>
        <Accion titulo="Editar" onPress={()=>abrir(m.tipo,m)}/><Accion titulo="Eliminar" disabled={ocupado} onPress={()=>Alert.alert('¿Eliminar el movimiento?',mxn.format(m.cantidad),[
          {text:'Cancelar',style:'cancel'},{text:'Eliminar',style:'destructive',onPress:()=>{void ejecutar(async()=>{await api.delete('movimientos/'+m.idMovimiento);});}}
        ])}/></View>)}
      {pagina>1&&<Accion titulo="Anterior" onPress={()=>setPagina(pagina-1)}/>}
      {pagina*20<total&&<Accion titulo="Siguiente" onPress={()=>setPagina(pagina+1)}/>}
    </>}</Pantalla>;
}