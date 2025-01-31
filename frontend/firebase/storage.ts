import { initializeApp } from "firebase/app";
import { FirebaseStorage, getStorage, ref, uploadBytes, UploadResult } from "firebase/storage";

const firebaseConfig = {
    apiKey: "AIzaSyCbo5BHBJKwhVblw_8RALdzKWnhCxyHGvI",
    authDomain: "huro-1b182.firebaseapp.com",
    projectId: "huro-1b182",
    storageBucket: "huro-1b182.firebasestorage.app",
    messagingSenderId: "155701458557",
    appId: "1:155701458557:web:c930b0728f08f7b66b2314"
  };


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


  // MODELS

  export enum FileUploadType {
    algorithm = 'algorithms',
    model = 'models',
  }

  export interface FileUpload { 
    file: (File | Blob),
    name: string,
    type: FileUploadType,
    OnUploadComplete?: (result?: UploadResult) => void,
  }
}
