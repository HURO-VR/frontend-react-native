import { Button, Text, View } from "react-native";
import FileUpload from "./components/fileUpload";
import { useEffect, useState } from "react";
import { DefaultStyles } from "./styles/DefaultStyles";
import TextStyles from "./styles/textStyles";
import { FBStorage } from "@/firebase/storage";
import { v4 as uuid } from "uuid";
import { TextInput } from "react-native-gesture-handler";
import { useRouter } from "expo-router";

export default function SimulationCreation() {

  const [uploadTrigger, setUploadTrigger] = useState(false);
  const simulationID = uuid();
  const [filesUploaded, setFilesUploaded] = useState(0);
  const [simulationName, onChangeText] = useState(simulationID);
  const [algorithmName, setAlgorithmName] = useState("Example");


  const TOTAL_FILES_UPLOAD = 2;

  useEffect(() => {
    if (filesUploaded === TOTAL_FILES_UPLOAD) {
      FBStorage.uploadSimulationMetaData("TEST_ORG", simulationID, simulationName, algorithmName).then(() => {
        useRouter().push("/view_simulations");
      });
    }
  }, [filesUploaded]);


  return (
    <View
      style={{
        flex: 1,
        justifyContent: "center",
        alignItems: "center",
      }}
    >
      <Text style={TextStyles.h3}>Create a Simulation</Text>
      <Text style={TextStyles.subtitle}>
        Upload your algorithm to create a new simulation.
      </Text>

      <View style={{ marginVertical: 10 }} />

      <Text style={TextStyles.h6}>Simulation Name: </Text>

      <TextInput
          style={{...DefaultStyles.input, width: "30%"}}
          onChangeText={onChangeText}
          value={simulationName}
        />
      <Text style={TextStyles.h6}>Algorithm Name: </Text>

    <TextInput
        style={{...DefaultStyles.input, width: "30%"}}
        onChangeText={setAlgorithmName}
        value={algorithmName}
        maxLength={50}
      />

      <FileUpload 
        onUploadComplete={() => setFilesUploaded(filesUploaded + 1)}
        maxSize={5 * 1024 * 1024} // 5MB
        uploadTrigger={uploadTrigger}
        title="Algorithm"
        fileType={FBStorage.FileUploadType.algorithm}
        simulationID={simulationID}
      />

      <FileUpload 
        onUploadComplete={() => setFilesUploaded(filesUploaded + 1)}
        maxSize={5 * 1024 * 1024} // 5MB
        uploadTrigger={uploadTrigger}
        title="3D Model"
        fileType={FBStorage.FileUploadType.algorithm}
        simulationID={simulationID}
      />


      <Text style={TextStyles.subtitle}>3D Model will default to sphere if left empty.</Text>
      <View style={{ marginVertical: 20 }} />
      <Button 
        title="Create Simulation"
        onPress={() => {
          if (algorithmName.length < 5) return;
          setUploadTrigger(true)}}
      />
    </View>
  );
}
