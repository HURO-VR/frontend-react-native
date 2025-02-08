import { Text, View, TextInput, TouchableOpacity, StyleSheet } from "react-native";
import FileUpload from "./components/fileUpload";
import { useEffect, useState } from "react";
import { DefaultStyles } from "./styles/DefaultStyles";
import TextStyles from "./styles/textStyles";
import { FBStorage } from "@/firebase/storage";
import { v4 as uuid } from "uuid";
import { useRouter } from "expo-router";

export default function SimulationCreation() {
  const [uploadTrigger, setUploadTrigger] = useState(false);
  const [simulationID, setSimulationID] = useState(uuid());
  const [filesUploaded, setFilesUploaded] = useState(false);
  const [algorithmName, setAlgorithmName] = useState("");
  const [algorithmFileName, setAlgorithmFileName] = useState(""); // Store uploaded file name
  const [simulationName, setSimulationName] = useState("");
  const [algorithmError, setAlgorithmError] = useState("");
  const [modelError, setModelError] = useState("");

  const router = useRouter();

  const getTimestampPrefix = () => {
    const now = new Date();
    return now.toISOString().replace(/[-:T]/g, "").slice(0, 13); // YYYYMMDD-HHMM
  };

  useEffect(() => {
    setSimulationName(algorithmName + "--" + getTimestampPrefix());
  }, [algorithmName]);

  const uploadMetaData = async () => {
    let done = await FBStorage.uploadSimulationMetaData(
      "TEST_ORG",
      {ID: simulationID,
      name: simulationName,
      algorithmFilename: algorithmFileName,
      dateCreated: new Date().toISOString(),
      environmentName: "empty-room"
    } // Use the actual uploaded ONNX file name
    )
      .then(() => true)
      .catch((e) => {
        window.alert("Error uploading simulation metadata: " + e);
        return false;
      });

    if (done) router.push("/view_simulations");
  };

  useEffect(() => {
    if (filesUploaded && algorithmFileName.length > 0) {
      uploadMetaData();
      console.log("Metadata uploaded successfully!");
    }
  }, [filesUploaded, algorithmFileName]);


  return (
    <View style={styles.container}>
      <Text style={TextStyles.h3}>Create New Simulation</Text>
      <Text style={TextStyles.subtitle}>
        Upload your custom algorithm and model to generate a new simulation.
      </Text>

      <View style={{ marginVertical: 10 }} />

      {/* Algorithm Name Input (User-Defined) */}
      <Text style={TextStyles.h6}>Algorithm Name:</Text>
      <TextInput
        style={{ ...DefaultStyles.input, width: "50%", backgroundColor: "#fff" }}
        onChangeText={setAlgorithmName}
        value={algorithmName}
        maxLength={50}
      />

      <View style={{ marginVertical: 15 }} />
      <Text style={TextStyles.subtitle}>
        1. Upload your robot algorithm [Supported formats: .onnx] (*Required):
      </Text>

      {/* File Upload for Algorithm (Updates algorithmFileName) */}
      <FileUpload
        onUploadComplete={(fileName) => {
          console.log("Uploaded file:", fileName);
          setFilesUploaded(true); // Signal that all files are uploaded
        }}
        onFilePicked={(fileName) => {
            // Ensure fileName is a valid string before updating state
          if (fileName && typeof fileName === "string" && fileName.toLowerCase().endsWith(".onnx")) {
            setAlgorithmFileName(fileName); // Store uploaded ONNX file name 
            console.log("Algorithm file name set to:", fileName);
            setAlgorithmError(""); // Clear any previous error
            return true;
          } else {
            console.warn("Invalid file name received:", fileName);
            setAlgorithmError("Please upload a valid ONNX file.");
            return false;
          }
        }}
        maxSize={5 * 1024 * 1024} // 5MB
        uploadTrigger={uploadTrigger}
        fileType={FBStorage.FileUploadType.algorithm}
        simulationID={simulationID}
      />
      {algorithmError.length > 0 && <Text style={{ color: "red" }}>{algorithmError}</Text>}


      {/* Model Upload (Optional) */}
      <View style={{ marginVertical: 10 }} />
        <Text style={TextStyles.subtitle}>
          2. Upload your 3D robot model [Default: sphere] (Optional):
        </Text>
      <FileUpload
        onUploadComplete={() => {}}
        onFilePicked={(filename) => {
          if (filename.split(".").pop() === "glb") {
            setModelError(""); // Clear any previous error
            return true;
          } else {
            setModelError("Please upload a valid GLB file.");
            return false;
          }
        }}
        maxSize={5 * 1024 * 1024} // 5MB
        uploadTrigger={uploadTrigger}
        fileType={FBStorage.FileUploadType.model}
        simulationID={simulationID}
      />
      {modelError.length > 0 && <Text style={{ color: "red" }}>{modelError}</Text>}

      <View style={{ marginVertical: 10 }} />

      {/* Auto-filled Simulation Name (Non-editable) */}
      <Text style={TextStyles.h6}>Simulation Name:</Text>
      <TextInput
        style={{ ...DefaultStyles.input, width: "50%", backgroundColor: "#f0f0f0" }}
        value={simulationName}
        editable={false}
      />

      <View style={{ marginVertical: 15 }} />

      {/* Generate Simulation Button */}
      <TouchableOpacity
        onPress={() => {
          if (algorithmName.length < 3 || algorithmFileName.length < 3) {
            window.alert("Please enter an Algorithm Name and upload an ONNX file.");
            return;
          }
          setUploadTrigger(true);
        }}
        style={[styles.generateButton, (algorithmName.length < 3 || algorithmFileName.length < 3) && styles.disabledButton]}
        disabled={algorithmName.length < 3 || algorithmFileName.length < 3}
        activeOpacity={0.7}
      >
        <Text style={styles.generateButtonText}>Generate Simulation</Text>
      </TouchableOpacity>
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    justifyContent: "center",
    alignItems: "center",
  },
  generateButton: {
    backgroundColor: "#000000",
    paddingVertical: 10,
    paddingHorizontal: 25,
    borderRadius: 5,
    alignItems: "center",
    justifyContent: "center",
    width: "50%",
    elevation: 5,
    shadowColor: "#000",
    shadowOffset: { width: 0, height: 4 },
    shadowOpacity: 0.3,
    shadowRadius: 5,
  },
  generateButtonText: {
    color: "#FFFFFF",
    fontSize: 18,
    fontWeight: "700",
    textTransform: "uppercase",
    letterSpacing: 1,
  },
  disabledButton: {
    backgroundColor: "#444",
    opacity: 0.5,
  },
});