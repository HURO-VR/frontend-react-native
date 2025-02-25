import { Slot, Stack, Tabs } from "expo-router";

export default function RootLayout() {
  return   <Stack
              screenOptions={{
                headerShown: false, // This removes headers for all screens
              }}
            />
}
