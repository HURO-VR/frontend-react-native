import { Button, Text, View, Image, StyleSheet } from "react-native";
import FileUpload from "./components/fileUpload";
import { useState } from "react";
import { DefaultStyles } from "./styles/DefaultStyles";
import { Link } from "expo-router";
import TextStyles from "./styles/textStyles";

export default function Index() {

  
  return (
    <View style={styles.container}>
      {/* Header */}
      <View style={styles.header}>
        <Image 
          source={require("../assets/images/huro_logo.png")}  // Replace with your actual logo path
          style={styles.logo}
        />
        {/* <Text style={styles.headerText}>HURO</Text> */}
      </View>

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

const styles = StyleSheet.create({
  container: {
    flex: 1,
    alignItems: "center",
    backgroundColor: "#fff",
    paddingVertical: 200,
  },
  header: {
    flexDirection: "row",
    alignItems: "center",
    justifyContent: "center",
    width: "100%",
    paddingVertical: 15,
    backgroundColor: "black", // Customize color
    position: "absolute",
    top: 0,
    left: 0,
    right: 0,
    zIndex: 1000, // Ensures it stays on top
  },
  logo: {
    marginRight: 10, // Space between logo and text
  },
  headerText: {
    fontSize: 20,
    color: "white",
    fontWeight: "bold",
  },
  content: {
    flex: 1,
    justifyContent: "center",
    alignItems: "center",
    width: "100%",
    paddingTop: 70, // Space for the fixed header
  },
});