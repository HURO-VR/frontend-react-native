// __tests__/app/create_simulation.test.tsx

import React from 'react';
import { render, fireEvent, waitFor } from '@testing-library/react-native';
import CreateSimulation from '../../app/create_simulation';

// Mock navigation
const mockNavigation = {
  navigate: jest.fn(),
};

jest.mock('@react-navigation/native', () => ({
  useNavigation: () => mockNavigation,
}));

describe('CreateSimulation Screen', () => {
  it('validates form inputs', async () => {
    const { getByPlaceholderText, getByText } = render(<CreateSimulation />);
    
    const nameInput = getByPlaceholderText('Simulation Name');
    fireEvent.changeText(nameInput, '');
    
    const submitButton = getByText('Create');
    fireEvent.press(submitButton);
    
    await waitFor(() => {
      expect(getByText('Name is required')).toBeTruthy();
    });
  });
});