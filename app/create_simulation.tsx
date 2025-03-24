import { Text, View, TextInput, TouchableOpacity, StyleSheet, Image } from "react-native";
import FileUpload from "./components/fileUpload";
import { useEffect, useState } from "react";
import { DefaultStyles } from "./styles/DefaultStyles";
import TextStyles from "./styles/textStyles";
import { FBStorage } from "@/firebase/storage";
import { v4 as uuid } from "uuid";
import { useLocalSearchParams, useRouter } from "expo-router";
import CustomDropdown from "./components/dropdown";
import DropdownMenu from "./components/dropdown";
import { AcceptedFileTypes, FileUploadType, EnvImages, EnvironmentTypes } from "@/firebase/models";
import { LocalRouteParamsContext } from "expo-router/build/Route";

export default function SimulationCreation() {
  const [uploadTrigger, setUploadTrigger] = useState(false);
  const [simulationID, setSimulationID] = useState(uuid());
  const [filesUploaded, setFilesUploaded] = useState(false);
  const [algorithmName, setAlgorithmName] = useState("");
  const [algorithmFileName, setAlgorithmFileName] = useState(""); // Store uploaded file name
  const [simulationName, setSimulationName] = useState("");
  const [environment, setEnvironment] = useState(EnvironmentTypes.emptyRoom);
  const [algorithmError, setAlgorithmError] = useState("");
  const [modelError, setModelError] = useState("");
  const [envJPG, setEnvJPG] = useState(EnvImages[environment]);

  const router = useRouter();
  const {org_id } = useLocalSearchParams()
  const getTimestampPrefix = () => {
    const now = new Date();
    return now.toISOString().replace(/[-:T]/g, "").slice(0, 13); // YYYYMMDD-HHMM
  };

  useEffect(() => {
    setSimulationName(algorithmName);
  }, [algorithmName]);

  useEffect(() => {
    setEnvJPG(EnvImages[environment]);  
  }, [environment]);

  const uploadMetaData = async () => {
    let done = await FBStorage.uploadSimulationMetaData(
      org_id as string,
      {
        ID: simulationID,
        name: simulationName,
        algorithmFilename: algorithmFileName,
        dateCreated: new Date().toISOString(),
        environmentName: environment,
        runs: 0
    } // Use the actual uploaded ONNX file name
    )
      .then(() => true)
      .catch((e) => {
        window.alert("Error uploading simulation metadata: " + e);
        return false;
      });

    if (done) router.push("/home");
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

      {/* User Defined Options Section */}
      <View style={{flexDirection: "row", alignItems: "center", justifyContent: "space-between", width: "90%", marginHorizontal: 20}}>

        {/* Algorithm Name and File Upload Section */}
        <View style={{flex: 1, marginRight: 10, alignItems: "flex-end"}}>
          {/* Algorithm Name Input (User-Defined) */}
          <Text style={TextStyles.h6}>Algorithm Name:</Text>
          <TextInput
            style={{ ...DefaultStyles.input, width: "50%", backgroundColor: "#fff" }}
            onChangeText={setAlgorithmName}
            value={algorithmName}
            placeholder="Enter Algorithm Name"
            maxLength={50}
          />

      {/* File Upload for Algorithm (Updates algorithmFileName) */}
      <FileUpload
        onUploadComplete={(fileName) => {
          console.log("Uploaded file:", fileName);
          setFilesUploaded(true); // Signal that all files are uploaded
        }}
        onFilePicked={(fileName) => {
          // Ensure fileName is a valid string before updating state
          if (AcceptedFileTypes.checkFilename(fileName, FileUploadType.algorithm)) {
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
        fileType={FileUploadType.algorithm}
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
          if (AcceptedFileTypes.checkFilename(filename, FileUploadType.model)) {
            setModelError(""); // Clear any previous error
            return true;
          } else {
            setModelError("Please upload a valid GLB file.");
            return false;
          }
        }}
        maxSize={5 * 1024 * 1024} // 5MB
        uploadTrigger={uploadTrigger}
        fileType={FileUploadType.model}
        simulationID={simulationID}
      />
      {modelError.length > 0 && <Text style={{ color: "red" }}>{modelError}</Text>}
        </View>
        {/* Environment Selection Section */}
        <View style={{flex: 1, marginLeft: 10, alignItems: "flex-start"}}>
            <Text style={TextStyles.h6}>Environment:</Text>
            <DropdownMenu
              options={Object.values(EnvironmentTypes)}
              defaultValue={Object.values(EnvironmentTypes)[0]}
              onSelect={(option) => {
                setEnvironment(option as EnvironmentTypes);
              }}
            />
            <Image 
              source={envJPG} 
              style={{ width: "40%", marginTop: 20 }} 
            />
        </View>
      </View>

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