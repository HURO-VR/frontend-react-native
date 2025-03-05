module.exports = {
    preset: 'jest-expo',
    setupFilesAfterEnv: ['<rootDir>/__tests__/setup.ts'],
    transformIgnorePatterns: [
      'node_modules/(?!(firebase|@firebase|jest-)?react-native|@react-native(-community)?|expo(nent)?|@expo(nent)?/.*|@expo-google-fonts/.*|react-navigation|@react-navigation/.*|@unimodules/.*|unimodules|sentry-expo|native-base|react-native-svg)',
    ],
    moduleFileExtensions: ['ts', 'tsx', 'js', 'jsx'],
    moduleNameMapper: {
      '^@/(.*)$': '<rootDir>/$1'
    },
    testEnvironment: 'jsdom',
    transform: {
      '^.+\\.(js|jsx|ts|tsx)$': ['babel-jest', { configFile: './babel.config.js' }]
    },
    setupFiles: ['<rootDir>/jest.setup.js']
}
