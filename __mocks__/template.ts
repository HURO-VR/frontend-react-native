const NameMock = () => jest.mock("npm_package", () => ({
    functionName: jest.fn().mockReturnValue("Mock return if needed")
  }));

  export default NameMock