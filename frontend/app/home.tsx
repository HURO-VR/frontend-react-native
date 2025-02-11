import { Text, View, Image, StyleSheet } from "react-native";
import { useEffect, useState } from "react";
import { Link, Redirect, useRootNavigationState, useRouter } from "expo-router";
import TextStyles from "./styles/textStyles";
import { FBAuth } from "@/firebase/auth";

export default function Home() {
  const [loggedIn, setLogin] = useState(FBAuth.isSignedIn())
  const rootNavigationState = useRootNavigationState();

  useEffect(() => {
    
  }, [])

  
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

    <Text>
      <b>Welcome to HURO.</b>
    </Text>
    
    <View style={{ marginVertical: 10 }} />
    <Text>
      Test your human-robot interactive algorithms in a real-time VR simulation environment.
    </Text>

    <View style={{ marginVertical: 10 }} />
    <Text> 
      Click below to get started:
    </Text>

    <View style={{ marginVertical: loggedIn ? 50 : 10 }} />
    {loggedIn ? 
    <>
    {/* Logged In Links */ }
    <Link href="/create_simulation"
      style={{
        color: 'black',
        // textDecorationLine: 'underline',
        ...TextStyles.h4,
        marginVertical: 10
       }}>Create a Simulation</Link>
    
    <Link href="/view_simulations"
      style={{
        color: 'black',
        // textDecorationLine: 'underline',
        ...TextStyles.h4,
        marginVertical: 10
       }}>View Simulations</Link>
    
    <Link href="/view_organization"
      style={{
        color: 'black',
        // textDecorationLine: 'underline',
        ...TextStyles.h4,
        marginVertical: 10
       }}>View Organization</Link>
      </> :
      <>
      {/* Logged Out Links */ }
        <Link href="/login"
        style={{
          color: 'black',
          // textDecorationLine: 'underline',
          ...TextStyles.h4,
          marginVertical: 10
          }}>Login</Link>
      </>
      }
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