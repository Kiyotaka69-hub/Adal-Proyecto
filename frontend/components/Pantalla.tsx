import { Button, ScrollView, Text, TextInput, type TextInputProps } from 'react-native';
import type { PropsWithChildren } from 'react';
export const vino='#751d39';
export function Pantalla({titulo,children}:PropsWithChildren<{titulo:string}>) {
  return <ScrollView keyboardShouldPersistTaps="handled" contentContainerStyle={{flexGrow:1,padding:24,gap:16,backgroundColor:'#fff'}}>
    <Text accessibilityRole="header" style={{fontSize:30,fontWeight:'700',color:vino}}>{titulo}</Text>{children}</ScrollView>;
}
export function Campo({label,...props}:TextInputProps & {label:string}) {
  return <><Text>{label}</Text><TextInput accessibilityLabel={label} style={{minHeight:48,borderColor:'#706568',borderWidth:1,borderRadius:8,paddingHorizontal:12,color:'#241920'}} {...props}/></>;
}
export function Accion({titulo,onPress,disabled}:{titulo:string;onPress:()=>void;disabled?:boolean}) {
  return <Button title={titulo} onPress={onPress} disabled={disabled} color={vino}/>;
}