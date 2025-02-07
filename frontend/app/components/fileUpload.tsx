import React, { useEffect, useState } from 'react';
import {
  View,
  Text,
  TouchableOpacity,
  StyleSheet,
  ActivityIndicator,
  Alert,
  Platform
} from 'react-native';
import * as DocumentPicker from 'expo-document-picker';
import { FBStorage } from '@/firebase/storage';
import TextStyles from '../styles/textStyles';

interface FileUploadProps {
    onUploadComplete?: (filename: string) => void;
    onFilePicked?: (filename: string) => void;
    maxSize?: number;
    allowedTypes?: string[];
    uploadTrigger?: boolean;
    title?: string
    fileType: FBStorage.FileUploadType
    simulationID: string
}

const FileUpload = ({ 
  onUploadComplete, 
  maxSize = 10 * 1024 * 1024, 
  allowedTypes = ['*/*'], 
  uploadTrigger, 
  title, 
  fileType, 
  simulationID ,
  onFilePicked
}: FileUploadProps) => {
  const [uploading, setUploading] = useState(false);
  const [fileName, setFileName] = useState('');
  const [uri, setUri] = useState('');

  useEffect(() => {
    if (uploadTrigger) uploadFile(uri);
  }, [uploadTrigger]);

  const checkFileSize = (size: number) => {
    if (size && size > maxSize) {
      let message = `File size must be less than ${maxSize / 1024 / 1024}MB`;
      console.log('File too large');
      if (Platform.OS === 'web') {
          window.alert(message);
      } else {
          Alert.alert(
              'File Too Large',
              message
          );
      }
      return false;
    }
    return true;
  };

  const pickDocument = async () => {
    try {
      const result = await DocumentPicker.getDocumentAsync({
        type: allowedTypes,
        copyToCacheDirectory: true,
        multiple: false,
      });
  
      if (!result.canceled) {
        const file = result.assets[0]; // Extract file object safely
        const fileName = file?.name ?? ""; // Ensure fileName is a string
  
        setFileName(fileName);
        setUri(file.uri);
  
        console.log("Selected file:", fileName, file.uri);
        onFilePicked && onFilePicked(fileName);
      }
    } catch (err) {
      console.error("Error picking document:", err);
      window.alert("Failed to pick document");
    }
  };  

  const uploadFile = async (uri: string) => {
    setUploading(true);
    try {
      const response = await fetch(uri);
      const blob = await response.blob();

      await FBStorage.uploadSimulationFile({
        file: blob, 
        name: fileName,
        type: fileType, 
        simulationID: simulationID
      }, 'TEST_ORG');

      onUploadComplete && onUploadComplete(fileName);
    } catch (error) {
      console.error('Upload error:', error);
      window.alert('Failed to upload file');
    } finally {
      setUploading(false);
    }
  };

  return (
    <View style={styles.container}>
      <Text style={{ paddingVertical: 5, ...TextStyles.h6 }}>{title}</Text>
      {fileName && <Text style={styles.fileName}>{fileName}</Text>}

      <TouchableOpacity 
        onPress={pickDocument} 
        style={[styles.blackButton, uploading && styles.disabledButton]} 
        disabled={uploading}
      >
        <Text style={styles.buttonText}>
          {uploading ? 'Uploading...' : 'Select File'}
        </Text>
      </TouchableOpacity>

      {uploading && (
        <View style={styles.progressContainer}>
          <ActivityIndicator size="small" color="#ffffff" />
          <Text style={styles.progressText}>Uploading...</Text>
        </View>
      )}
    </View>
  );
};

const styles = StyleSheet.create({
  container: {
    padding: 10,
  },
  blackButton: {
    backgroundColor: "#000000",
    paddingVertical: 12,
    paddingHorizontal: 20,
    borderRadius: 8,
    alignItems: "center",
    justifyContent: "center",
    marginTop: 0,
  },
  disabledButton: {
    opacity: 0.5,
  },
  buttonText: {
    color: "#FFFFFF",
    fontSize: 14,
    fontWeight: "600",
  },
  fileName: {
    marginTop: 10,
    fontSize: 14,
    color: "#666666",
  },
  progressContainer: {
    flexDirection: "row",
    alignItems: "center",
    marginTop: 10,
    gap: 10,
  },
  progressText: {
    fontSize: 14,
    color: "#666666",
  },
});

export default FileUpload;
