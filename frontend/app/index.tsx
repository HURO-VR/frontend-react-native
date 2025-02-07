import { Button, StyleSheet, Text, View } from "react-native";
import FileUpload from "./components/fileUpload";
import { useState } from "react";
import { DefaultStyles } from "./styles/DefaultStyles";
import { Link } from "expo-router";
import TextStyles from "./styles/textStyles";

export default function Index() {
  
  return (
    <View
      style={{
        flex: 1,
        justifyContent: "center",
        alignItems: "center",
      }}
    >
      <Text style={{...TextStyles.h1, marginVertical: 10}}>Options:</Text>
      <Link href="/create_simulation"
      style={{
        color: 'blue',
        textDecorationLine: 'underline',
        ...TextStyles.h4
       }}>Create Simulation</Link>


       <Link href="/view_simulations"
      style={{
        color: 'blue',
        textDecorationLine: 'underline',
        ...TextStyles.h4,
        marginVertical: 10
       }}>View Simulations</Link>


    <Link href="/create_simulation"
      style={{
        color: 'blue',
        textDecorationLine: 'underline',
        ...TextStyles.h4
       }}>View Organization</Link>
    </View>
  );
}
