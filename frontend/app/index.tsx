import { FBAuth } from '@/firebase/auth';
import { useRootNavigationState, Redirect } from 'expo-router';
import { useEffect, useState } from 'react';
import { View } from 'react-native';

export default function App() {
  const rootNavigationState = useRootNavigationState();

  if (!rootNavigationState?.key) return null;
  
  
  return FBAuth.isSignedIn() != null ? <Redirect href={"/home"} /> : <Redirect href={"/login"} />
}