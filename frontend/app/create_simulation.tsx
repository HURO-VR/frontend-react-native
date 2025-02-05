import { Button, Text, View, TextInput } from "react-native";
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

  // Function to generate timestamp prefix
  const getTimestampPrefix = () => {
    const now = new Date();
    return now.toISOString().replace(/[-:T]/g, "").slice(0, 13); // YYYYMMDD-HHMM
  };

  // Function to check existing simulations and determine trial number
  const fetchAndSetTrialNumber = async (algoName) => {
    const timestampPrefix = getTimestampPrefix();
    const baseName = `${algoName}-${timestampPrefix}`;
    
    try {
      // Fetch existing simulations with this prefix
      const existingSimulations = await FBStorage.getExistingSimulations(baseName); // Assume this function returns an array of existing names
      console.log("Existing Simulations: ", existingSimulations);

      // Find the highest trial number
      let maxTrial = 0;
      existingSimulations.forEach((simName) => {
        const match = simName.match(/-(\d+)$/); // Extract last number
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

  // Update simulation name when algorithm name changes
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
    if (filesUploaded === true) {
      onUploadComplete();
    }
  }, [filesUploaded]);

  return (
    <View style={styles.container}>
      <Text style={TextStyles.h3}>Create a Simulation</Text>
      <Text style={TextStyles.subtitle}>
        Upload your algorithm to create a new simulation.
      </Text>

      <View style={{ marginVertical: 10 }} />

      {/* Algorithm Name Input */}
      <Text style={TextStyles.h6}>Algorithm Name:</Text>
      <TextInput
        style={{ ...DefaultStyles.input, width: "50%" }}
        onChangeText={setAlgorithmName}
        value={algorithmName}
        maxLength={50}
      />

      <View style={{ marginVertical: 10 }} />

      {/* Auto-filled Simulation Name (Non-editable) */}
      <Text style={TextStyles.h6}>Simulation Name:</Text>
      <TextInput
        style={{ ...DefaultStyles.input, width: "50%", backgroundColor: "#f0f0f0" }}
        value={simulationName}
        editable={false} // Make it read-only
      />

      <View style={{ marginVertical: 10 }} />

      {/* File Upload Sections */}
      <FileUpload
        onUploadComplete={() => setFilesUploaded(true)}
        maxSize={5 * 1024 * 1024} // 5MB
        uploadTrigger={uploadTrigger}
        title="Algorithm"
        fileType={FBStorage.FileUploadType.algorithm}
        simulationID={simulationID}
      />

      <FileUpload
        onUploadComplete={() => {}}
        maxSize={5 * 1024 * 1024} // 5MB
        uploadTrigger={uploadTrigger}
        title="3D Model"
        fileType={FBStorage.FileUploadType.model}
        simulationID={simulationID}
      />

      <Text style={TextStyles.subtitle}>
        3D Model will default to a sphere if left empty.
      </Text>

      <View style={{ marginVertical: 20 }} />

      {/* Create Simulation Button */}
      <Button
        title="Create Simulation"
        onPress={() => {
          if (algorithmName.length < 5) return;
          setUploadTrigger(true);
        }}
      />
    </View>
  );
}

const styles = {
  container: {
    flex: 1,
    justifyContent: "center",
    alignItems: "center",
  },
};
