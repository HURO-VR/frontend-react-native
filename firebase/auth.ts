import { initializeApp } from "firebase/app";
import { firebaseConfig } from "./config";
import { createUserWithEmailAndPassword, getAuth, signInWithEmailAndPassword  } from "firebase/auth";
import {UserMetaData} from "./models"
import { FBStorage } from "./storage";
import { FBFirestore } from "./firestore";

export class FBAuth {
    static app = initializeApp(firebaseConfig);
    static auth = getAuth()

    public static isSignedIn(): string | null {
        if (this.auth.currentUser == null) {
            return null;
        }
        return this.auth.currentUser.uid;
    }

    public static getUID() {
        return this.auth.currentUser?.uid;
    }

    public static GetUserMetaData(uid: string) {
        return FBFirestore.getFSDoc(`users/${uid}`)
    }

    static initializeUser(user: UserMetaData) {
        return FBFirestore.firestoreDocUpload(user.uid, `users`, user)
    }

    public static async createUser(email: string, password: string, name: string) {
        return createUserWithEmailAndPassword(this.auth, email, password)
        .then(async (userCredential) => {
            // Signed up 
            const user = userCredential.user;
            const userMetadata = {
                uid: user.uid,
                name: name,
                email: user.email!,
                organizations: [],
                colleagues: []
            }
            await this.initializeUser(userMetadata)
            return userMetadata;
            // ...
        })
        .catch((error) => {
            throw error.message;
        });
    }

    public static async authenticate(email: string, password: string) {
        return signInWithEmailAndPassword(this.auth, email, password)
        .then(async (userCredential) => {
            // Signed in 
            const authUser = userCredential.user;
            const user = await FBFirestore.getFSDoc(`users/${authUser.uid}`) as UserMetaData
            return user;
        })
        .catch((error) => {
            throw error.message;
        });
    }
}