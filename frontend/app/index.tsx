import { Button, Text, View, Image, StyleSheet } from "react-native";
import FileUpload from "./components/fileUpload";
import { useState } from "react";
import { DefaultStyles } from "./styles/DefaultStyles";

export default function Index() {
  const [uploadTrigger, setUploadTrigger] = useState(false);

  const handleUploadComplete = () => {
    console.log("Upload complete");
    setUploadTrigger(false);
  };

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

      {/* Main Content */}
      <Text>Upload your Model</Text>
      <p>
        Supported formats: onnx
      </p>

      <FileUpload 
        onUploadComplete={handleUploadComplete}
        maxSize={5 * 1024 * 1024} // 5MB
        uploadTrigger={uploadTrigger}
      />

      <Button 
        title="Upload File"
        onPress={() => setUploadTrigger(true)}
      />
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