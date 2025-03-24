const firestoreMock = () => jest.mock("firebase/firestore", () => ({
  getFirestore: jest.fn().mockReturnValue(getFirestoreRet),
  collection: jest.fn(),
  doc: jest.fn(),
  setDoc: jest.fn().mockResolvedValue(true),
  getDoc: jest.fn().mockResolvedValue(mockDocSnapshot),
  getDocs: jest.fn().mockResolvedValue(mockQuerySnapshot),
  query: jest.fn(),
  where: jest.fn(),
  onSnapshot: jest.fn().mockImplementation((doc, callback) => {
    callback(mockDocSnapshot);
    return jest.fn(); // Returns unsubscribe function
  })
}));

// Mock document snapshot
const mockDocSnapshot = {
  data: jest.fn().mockReturnValue({
    name: "test-simulation",
    ID: "test-id",
    dateCreated: "2024-02-15",
    algorithmName: "test-algorithm",
    environmentName: "test-environment"
  })
};

// Mock query snapshot
const mockQuerySnapshot = {
  forEach: jest.fn((callback) => {
    callback(mockDocSnapshot);
  })
};

const getFirestoreRet = {
  // Add any needed firestore instance properties
};


export default firestoreMock