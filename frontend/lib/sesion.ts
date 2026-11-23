import axios, { isAxiosError } from 'axios';
import * as SecureStore from 'expo-secure-store';
import { useSyncExternalStore } from 'react';

export type Perfil = { idUsuario: number; nombre: string; correo: string; rol: 'Usuario' | 'Administrador' };
type Estado = { perfil: Perfil | null; cargando: boolean; error?: string };
const clave = 'moneytrack.jwt';
const url = process.env.EXPO_PUBLIC_API_URL;
const { create } = axios;
export const api = create({ baseURL: url, timeout: 10000 });
let jwt: string | null = null;
let estado: Estado = { perfil: null, cargando: true };
const listeners = new Set<() => void>();
function cambiar(s: Estado) { estado = s; listeners.forEach(fn => fn()); }
function suscribir(fn: () => void) { listeners.add(fn); return () => { listeners.delete(fn); }; }
export function useSesion() { return useSyncExternalStore(suscribir, () => estado, () => estado); }
api.interceptors.request.use(config => {
  if (!url?.startsWith('https://')) throw new Error('Configura EXPO_PUBLIC_API_URL con la dirección HTTPS de la API.');
  if (jwt) config.headers.Authorization = 'Bearer ' + jwt;
  return config;
});
api.interceptors.response.use(r => r, async error => {
  if (jwt && error.response?.status === 401 && error.config?.headers?.Authorization === 'Bearer ' + jwt) await salir();
  return Promise.reject(error);
});
export async function salir() {
  await SecureStore.deleteItemAsync(clave);
  jwt = null;
  cambiar({ perfil: null, cargando: false });
}
export async function restaurar() {
  cambiar({perfil:null,cargando:true});
  try {
    jwt = await SecureStore.getItemAsync(clave);
    const perfil = jwt ? (await api.get<Perfil>('auth/me')).data : null;
    cambiar({ perfil, cargando: false });
  } catch {
    cambiar({ perfil:null,cargando:false,error:jwt ? 'No se pudo comprobar la sesión.' : undefined });
  }
}
export async function entrar(correo:string,password:string) {
  const {data} = await api.post<{token:string;usuario:Perfil}>('auth/login',{correo,password});
  await SecureStore.setItemAsync(clave,data.token);
  jwt=data.token;
  cambiar({perfil:data.usuario,cargando:false});
}
export function mensaje(e:unknown) {
  if(isAxiosError(e)) {
    switch(e.response?.status) {
      case 401:return 'Verifica el correo y la contraseña.';
      case 403:return 'Tu cuenta carece de permiso para esta operación.';
      case 409:return 'El registro entra en conflicto con uno existente.';
      case 400:return e.response.data?.detail ?? 'Revisa los datos del formulario.';
      case 429:return 'Espera un minuto antes de volver a intentar.';
      default:return 'No se pudo completar la solicitud. Revisa la conexión.';
    }
  }
  return e instanceof Error ? e.message : 'Ocurrió un error.';
}