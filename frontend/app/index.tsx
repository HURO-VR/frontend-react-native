import { Button, Text, View } from "react-native";
import FileUpload from "./components/fileUpload";
import { useState } from "react";
import { DefaultStyles } from "./styles/DefaultStyles";

export default function Index() {

  const [uploadTrigger, setUploadTrigger] = useState(false);
  
  return (
    <View
      style={{
        flex: 1,
        justifyContent: "center",
        alignItems: "center",
      }}
    >
      <Text>File Upload Test</Text>

      <FileUpload 
        onUploadComplete={() => console.log('Upload complete')}
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
