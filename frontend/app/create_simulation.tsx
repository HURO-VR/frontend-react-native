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
  const simulationID = uuid();
  const [filesUploaded, setFilesUploaded] = useState(false);
  const [algorithmName, setAlgorithmName] = useState("Example");
  const [simulationName, setSimulationName] = useState("");
  const [trialNumber, setTrialNumber] = useState(1);

  const router = useRouter();

  const getTimestampPrefix = () => {
    const now = new Date();
    return now.toISOString().replace(/[-:T]/g, "").slice(0, 13); // YYYYMMDD-HHMM
  };

  const fetchAndSetTrialNumber = async (algoName) => {
    const timestampPrefix = getTimestampPrefix();
    const baseName = `${algoName}-${timestampPrefix}`;

    try {
      const existingSimulations = await FBStorage.getExistingSimulations(baseName);
      let maxTrial = 0;
      existingSimulations.forEach((simName) => {
        const match = simName.match(/-(\d+)$/);
        if (match) {
          const num = parseInt(match[1], 10);
          if (!isNaN(num) && num > maxTrial) {
            maxTrial = num;
          }
        }
      });

      setTrialNumber(maxTrial + 1);
      setSimulationName(`${baseName}-${maxTrial + 1}`);
    } catch (error) {
      console.error("Error fetching existing simulations:", error);
      setTrialNumber(1);
      setSimulationName(`${baseName}-1`);
    }
  };

  useEffect(() => {
    fetchAndSetTrialNumber(algorithmName);
  }, [algorithmName]);

  const onUploadComplete = async () => {
    let done = await FBStorage.uploadSimulationMetaData(
      "TEST_ORG",
      simulationID,
      simulationName,
      algorithmName
    )
      .then(() => true)
      .catch((e) => {
        window.alert("Error uploading simulation metadata: " + e);
        return false;
      });

    if (done) router.push("/view_simulations");
  };

  useEffect(() => {
    if (filesUploaded) {
      onUploadComplete();
    }
  }, [filesUploaded]);

  return (
    <View style={styles.container}>
      <Text style={TextStyles.h3}>Create a Simulation</Text>
      <Text style={TextStyles.subtitle}>
        Upload your custom algorithm and model to generate a new simulation.
      </Text>

      <View style={{ marginVertical: 10 }} />

      {/* Algorithm Name Input */}
      <Text style={TextStyles.h6}>Insert Algorithm Name:</Text>
      <TextInput
        style={{ ...DefaultStyles.input, width: "50%", backgroundColor: "#fff" }}
        onChangeText={setAlgorithmName}
        value={algorithmName}
        maxLength={50}
      />

      <View style={{ marginVertical: 15 }} />
      <Text style={TextStyles.subtitle}>
        Upload your robot algorithm [Current supported formats: onnx]:
      </Text>
      {/* File Upload Sections */}
      <FileUpload
        onUploadComplete={() => setFilesUploaded(true)}
        maxSize={5 * 1024 * 1024} // 5MB
        uploadTrigger={uploadTrigger}
        fileType={FBStorage.FileUploadType.algorithm}
        simulationID={simulationID}
      />
      <View style={{ marginVertical: 10 }} />
      <Text style={TextStyles.subtitle}>
        Upload your 3D robot model [Optional, default: sphere]:
      </Text>
      <FileUpload
        onUploadComplete={() => {}}
        maxSize={5 * 1024 * 1024} // 5MB
        uploadTrigger={uploadTrigger}
        fileType={FBStorage.FileUploadType.model}
        simulationID={simulationID}
      />
      <View style={{ marginVertical: 10 }} />

      {/* Auto-filled Simulation Name (Non-editable) */}
      <Text style={TextStyles.h6}>Simulation Name:</Text>
      <TextInput
        style={{ ...DefaultStyles.input, width: "50%", backgroundColor: "#f0f0f0" }}
        value={simulationName}
        editable={false} // Make it read-only
      />

      <View style={{ marginVertical: 20 }} />

      {/* Generate Simulation Button */}
      <TouchableOpacity
        onPress={() => {
          if (algorithmName.length < 5) return;
          setUploadTrigger(true);
        }}
        style={[styles.generateButton, algorithmName.length < 3 && styles.disabledButton]}
        disabled={algorithmName.length < 3 }
        activeOpacity={0.7}
      >
        <Text style={styles.generateButtonText}>Generate</Text>
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
    paddingVertical: 15,
    paddingHorizontal: 25,
    borderRadius: 10,
    alignItems: "center",
    justifyContent: "center",
    width: "50%",
    elevation: 5, // Adds shadow on Android
    shadowColor: "#000", // Adds shadow on iOS
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
    backgroundColor: "#444", // Dark gray when disabled
    opacity: 0.5,
  },
});

export default SimulationCreation;
