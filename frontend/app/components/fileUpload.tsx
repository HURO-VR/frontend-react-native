import React, { useEffect, useState } from 'react';
import {
  View,
  Text,
  TouchableOpacity,
  StyleSheet,
  ActivityIndicator,
  Alert,
  Platform,
  Button
} from 'react-native';
import * as DocumentPicker from 'expo-document-picker';
import { FBStorage } from '@/firebase/storage';
import TextStyles from '../styles/textStyles';

interface FileUploadProps {
    onUploadComplete?: () => void;
    maxSize?: number;
    allowedTypes?: string[];
    uploadTrigger?: boolean;
    title: string
    fileType: FBStorage.FileUploadType
    simulationID: string
}


// Allows users to upload files using the Expo DocumentPicker API.
// Single file upload only.
// Default max size of 10MB.
// Will upload when uploadTrigger - a stateful boolean - is set to true. Or automatically if uploadTrigger is undefined.

const FileUpload = ({ onUploadComplete, maxSize = 10 * 1024 * 1024, allowedTypes = ['*/*'], uploadTrigger, title, fileType, simulationID }: FileUploadProps) => {
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
  }

  const pickDocument = async () => {
    try {
      const result = await DocumentPicker.getDocumentAsync({
        type: allowedTypes,
        copyToCacheDirectory: true,
        multiple: false, // Allow only one file to be selected
      });

      if (result.canceled == false) {
        const { uri, name, size } = result.assets[0];
        
        // Check file size
        if (size && !checkFileSize(size)) return;

        setFileName(name);
        setUri(uri);

        console.log('Selected file:', name, uri);
        if (uploadTrigger == undefined) uploadFile(uri);
      } else {
        window.alert('Document picker cancelled');
      }
    } catch (err) {
      console.error('Error picking document:', err);
      window.alert('Failed to pick document');
    }
  };


  const uploadFile = async (uri: string) => {
    setUploading(true);

    try {
      // Handle upload based on platform
        const response = await fetch(uri);
        const blob = await response.blob();

        FBStorage.uploadSimulationFile({
          file: blob, 
          name: fileName,
          type: fileType, 
          OnUploadComplete: onUploadComplete,
          simulationID: simulationID
        }, 'TEST_ORG');

      onUploadComplete && onUploadComplete();
      window.alert('File uploaded successfully');

    } catch (error) {
      console.error('Upload error:', error);
      window.alert('Failed to upload file');
    } finally {
      setUploading(false);
    }
  };

  return (
    <View style={styles.container}>
        <Text style={{paddingVertical: 5, ...TextStyles.h6}}>{title}</Text>
      {fileName && <Text>{fileName}</Text>}
      <Button
        onPress={pickDocument}
        disabled={uploading}
        title={uploading ? 'Uploading...' : 'Select File'}
        
    />
      {uploading && (
        <View style={styles.progressContainer}>
          <ActivityIndicator size="small" color="#0000ff" />
        </View>
      )}
    </View>
  );
};

const styles = StyleSheet.create({
  container: {
    padding: 20,
  },
  buttonText: {
    color: '#FFFFFF',
    fontSize: 16,
    fontWeight: '600',
  },
  fileName: {
    marginTop: 10,
    fontSize: 14,
    color: '#666666',
  },
  progressContainer: {
    flexDirection: 'row',
    alignItems: 'center',
    marginTop: 10,
    gap: 10,
  },
  progressText: {
    fontSize: 14,
    color: '#666666',
  },
});

export default FileUpload;
