// __tests__/app/components/file_upload.test.tsx

// import React from 'react';
// import { render } from '@testing-library/react-native';
// import ConditionalView from '../../../app/components/ConditionalView';

// describe('ConditionalView', () => {
//   it('renders children when condition is true', () => {
//     const { getByText } = render(
//       <ConditionalView condition={true}>
//         <Text>Test Content</Text>
//       </ConditionalView>
//     );
//     expect(getByText('Test Content')).toBeTruthy();
//   });

//   it('does not render children when condition is false', () => {
//     const { queryByText } = render(
//       <ConditionalView condition={false}>
//         <Text>Test Content</Text>
//       </ConditionalView>
//     );
//     expect(queryByText('Test Content')).toBeNull();
//   });
// });

import React from 'react';
import { render, fireEvent, waitFor } from '@testing-library/react-native';
import { Text, View } from 'react-native';
import FileUpload from '../../../app/components/fileUpload';
import * as DocumentPicker from 'expo-document-picker';

// Mock the DocumentPicker result
const mockPickResult = {
  type: 'success',
  uri: 'file://test.pdf',
  name: 'test.pdf',
  size: 1024,
} as DocumentPicker.DocumentResult;

describe('FileUpload', () => {
  beforeEach(() => {
    jest.clearAllMocks();
    (DocumentPicker.getDocumentAsync as jest.Mock).mockResolvedValue(mockPickResult);
  });

  it('renders upload button', () => {
    const { getByText } = render(
      <FileUpload onFileSelect={() => {}} />
    );
    expect(getByText('Choose File')).toBeTruthy();
  });

  it('calls onFileSelect when file is picked', async () => {
    const onFileSelect = jest.fn();
    const { getByText } = render(
      <FileUpload onFileSelect={onFileSelect} />
    );

    const button = getByText('Choose File');
    fireEvent.press(button);

    await waitFor(() => {
      expect(DocumentPicker.getDocumentAsync).toHaveBeenCalled();
      expect(onFileSelect).toHaveBeenCalledWith(mockPickResult);
    });
  });

  it('shows error message when file selection fails', async () => {
    (DocumentPicker.getDocumentAsync as jest.Mock).mockResolvedValue({ type: 'cancel' });
    
    const { getByText, findByText } = render(
      <FileUpload onFileSelect={() => {}} />
    );

    fireEvent.press(getByText('Choose File'));

    await findByText('File selection cancelled');
  });
});