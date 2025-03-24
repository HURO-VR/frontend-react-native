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

  export async function firestoreDocUpload(docID: string, path: string, data: any, options?: SetOptions) {
    try {
      let db = getFBFirestore();
      if (options) await setDoc(doc(db, path, docID), data, options);
      else await setDoc(doc(db, path, docID), data);
      return true;

    } catch (e) {
      console.error("Error adding document: ", e);
      return false;
    }
  }

  export async function addMembersToOrg(org: Organization, memberIDs: string[]) {
      return firestoreDocUpload(org.id, "organizations", {
        members: [...org.members, ...memberIDs]
      }, {merge: true})
  }

  export async function updateDoc<T>(docID: string, path: string, data: Partial<T>) {
    return firestoreDocUpload(docID, path, data, {merge: true})
}


  export async function getFSDoc(path: string): Promise<any> {
    try {
      let db = getFBFirestore();
      return (await getDoc(doc(db, path)))?.data();
    } catch (e) {
      console.error("Error getting document: ", e);
    }
  }

  export async function createOrganization(name: string, admin: string, members: string[]) {
      let db = getFBFirestore();
      let id = v4()
      const org : Organization = {
          name: name,
          id: id,
          dateCreated: new Date().toISOString(),
          members: members,
          admins: [admin],
          simulations: []
      }
      await setDoc(doc(db, `organizations`, id), org); // Model: Organization
      return org;
  }

  export async function getCollection(path: string, options?: {field: string, operation: WhereFilterOp, value: string | string[]}) {
    let db = getFBFirestore();
    let data: any[] = []
    let q;
    if (options) q = query(collection(db, path), where(options.field, options.operation, options.value));
    else q = query(collection(db, path))
    const querySnapshot = await getDocs(q);
    querySnapshot.forEach((doc) => {
      data.push(doc.data() as SimulationMetaData);
    });
    return data;
  }


  export function subscribeToDoc(docID: string, path: string, onUpdate: (data: DocumentData | undefined) => void) {
    const unsub = onSnapshot(doc(getFirestore(), docID, path), (doc) => {
      onUpdate(doc.data());
    });
    return unsub;
  }

  export function subscribeToCollection(path: string, onUpdate: (data: any[] | undefined) => void) {
    const unsub = onSnapshot(collection(getFirestore(), path), (col) => {
      var list: any[] = []
      col.forEach((doc) => {
        list.push(doc.data());
      });
      if (list.length > 0) onUpdate(list);
    });
    return unsub;
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
