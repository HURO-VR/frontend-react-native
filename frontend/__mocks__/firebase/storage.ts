import { UploadResult } from "firebase/storage";

const getStorageRet = {
  // Add any needed storage instance properties
};

const storageMock = () => jest.mock("firebase/storage", () => ({
  getStorage: jest.fn().mockReturnValue(getStorageRet),
  ref: jest.fn(),
  uploadBytes: jest.fn().mockResolvedValue({
    metadata: { name: "test-file" },
    ref: { fullPath: "test/path" }
  } as UploadResult)
}));

export default storageMock
