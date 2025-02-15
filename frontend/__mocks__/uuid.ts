const uuidMock = () => jest.mock("uuid", () => ({
    v4: jest.fn().mockReturnValue("test-uuid-9543-8324")
  }));

  export default uuidMock