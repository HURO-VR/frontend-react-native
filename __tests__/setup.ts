// __tests__/setup.ts

import '@testing-library/jest-native/extend-expect';

// Mock Expo modules that might cause issues
jest.mock('expo-font');
jest.mock('expo-asset');

// Basic setup without external dependencies first
global.console = {
    ...console,
    // log: jest.fn(),  // Uncomment to suppress console.log during tests
    // warn: jest.fn(), // Uncomment to suppress console.warn during tests
    // error: jest.fn() // Uncomment to suppress console.error during tests
};
  
// Mock modules that might not be available
jest.mock('expo-font', () => ({
    ...jest.requireActual('expo-font'),
    useFonts: () => [true, null],
}));
  
jest.mock('expo-asset', () => ({
    Asset: {
      fromModule: () => ({ downloadAsync: jest.fn() }),
    },
}));
  
// Try to import @testing-library/jest-native if available
try {
    require('@testing-library/jest-native/extend-expect');
} catch (e) {
    console.warn('Optional dependency @testing-library/jest-native not found');
}
