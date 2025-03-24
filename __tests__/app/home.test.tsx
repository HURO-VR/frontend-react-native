// __tests__/app/components/home.test.tsx

import React from 'react';
import { render, fireEvent, waitFor } from '@testing-library/react-native';
import HomeScreen from '../../screens/Home';
  
describe('HomeScreen', () => {
    it('displays user data after fetching', async () => {
      const { getByText, findByText } = render(<HomeScreen />);
      
      // Check loading state
      expect(getByText('Loading...')).toBeTruthy();
      
      // Wait for data to load
      const userName = await findByText('John Doe');
      expect(userName).toBeTruthy();
    });
});