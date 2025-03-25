import { initializeApp } from "firebase/app";
import { getStorage, ref, uploadBytes, UploadResult } from "firebase/storage";
import { collection, doc, DocumentData, getDoc, getDocs, getFirestore, onSnapshot, query, setDoc, SetOptions, where, WhereFilterOp } from "firebase/firestore";
import { FileUploadType, Organization, SimulationMetaData } from "./models";
import { firebaseConfig } from "./config";
import { v4 } from "uuid";


export namespace FBStorage {
  // FUNCTIONS

  export function getFBStorage() {
    // Initialize Firebase
    const app = initializeApp(firebaseConfig);

    // Initialize Cloud Storage and get a reference to the service
    const storage = getStorage(app);
    return storage;
  }


  export function uploadFile(file: FileUpload, organization: string) {
    const storage = getFBStorage();
    const storageRef = ref(storage, `organizations/${organization}/${file.type}/${file.name}`);

    uploadBytes(storageRef, file.file).then((snapshot) => {
        console.log('Uploaded a blob or file!');
        if (file.OnUploadComplete) file.OnUploadComplete(snapshot);
    });
  }

  export async function uploadSimulationFile(file: FileUpload, organization: string) {
    const storage = getFBStorage();
    const storageRef = ref(storage, `organizations/${organization}/simulations/${file.simulationID}/${file.type}/${file.name}`);

    await uploadBytes(storageRef, file.file).then((snapshot) => {
        console.log('Uploaded a blob or file!');
        if (file.OnUploadComplete) file.OnUploadComplete(snapshot);
    });
  }



  // MODELS

  export interface FileUpload { 
    file: (File | Blob),
    name: string,
    type: FileUploadType,
    simulationID: string,
    OnUploadComplete?: (result?: UploadResult) => void,
  }
}
