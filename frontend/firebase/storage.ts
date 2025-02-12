import { initializeApp } from "firebase/app";
import { FirebaseStorage, getStorage, ref, uploadBytes, UploadResult } from "firebase/storage";
import { addDoc, collection, doc, getDoc, getDocs, getFirestore, query, setDoc } from "firebase/firestore";
import { FileUploadType, SimulationMetaData } from "./models";
import { firebaseConfig } from "./config";



export namespace FBStorage {
  // FUNCTIONS

  export function getFBStorage() {
    // Initialize Firebase
    const app = initializeApp(firebaseConfig);

    // Initialize Cloud Storage and get a reference to the service
    const storage = getStorage(app);
    return storage;
  }

  export function getFBFirestore() {
    const app = initializeApp(firebaseConfig);

    // Initialize Cloud Storage and get a reference to the service
    const firestore = getFirestore(app);
    return firestore;
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

  export async function uploadSimulationMetaData(org: string, metaData: SimulationMetaData) {
    try {
      let db = getFBFirestore();
      await setDoc(doc(db, `organizations/${org}/simulations`, metaData.ID), {
        name: metaData.name,
        ID: metaData.ID,
        dateCreated: metaData.dateCreated,
        algorithmName: metaData.algorithmFilename,
        environmentName: metaData.environmentName,
      }); // Model: SimulationMetaData
      return true;

    } catch (e) {
      console.error("Error adding document: ", e);
      return false;
    }
  }

  export async function getAllSimulationsMetaData(organization: string) {
    let db = getFBFirestore();
    let sims: SimulationMetaData[] = []
    const q = query(collection(db, `organizations/${organization}/simulations`));
    const querySnapshot = await getDocs(q);
    querySnapshot.forEach((doc) => {
      sims.push(doc.data() as SimulationMetaData);
    });
    return sims;
  }

  export async function firestoreDocUpload(docID: string, path: string, data: any) {
    try {
      let db = getFBFirestore();
      await setDoc(doc(db, path, docID), data); // Model: SimulationMetaData
      return true;

    } catch (e) {
      console.error("Error adding document: ", e);
      return false;
    }
  }

  export async function getFSDoc(path: string): Promise<any> {
    try {
      let db = getFBFirestore();
      return (await getDoc(doc(db, path)))?.data();
    } catch (e) {
      console.error("Error getting document: ", e);
    }
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
