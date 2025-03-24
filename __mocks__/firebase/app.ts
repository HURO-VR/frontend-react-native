const appMock = () => jest.mock("firebase/app", () => ({
    initializeApp: jest.fn()
  }));

  export default appMock