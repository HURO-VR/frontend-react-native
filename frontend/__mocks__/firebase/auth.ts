const authMock = () => jest.mock("firebase/auth", () => ({
    signInWithEmailAndPassword: jest.fn(),
    createUserWithEmailAndPassword: jest.fn(),
    signOut: jest.fn(),
    sendPasswordResetEmail: jest.fn(),
    getAuth: jest.fn().mockReturnValue((getAuthRet))
  }));

const getAuthRet = {
    currentUser: {
        uid: "test-user"
    }
}

export default authMock