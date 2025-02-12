import { initializeApp } from "firebase/app";
import { firebaseConfig } from "./config";
import { createUserWithEmailAndPassword, getAuth, signInWithEmailAndPassword } from "firebase/auth";
import {UserMetaData} from "./models"
import { FBStorage } from "./storage";

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
        return FBStorage.getFSDoc(`users/${uid}`)
    }

    static initializeUser(user: UserMetaData) {
        return FBStorage.firestoreDocUpload(user.uid, `users`, user)
    }

    public static createUser(email: string, password: string, name: string) {
        return createUserWithEmailAndPassword(this.auth, email, password)
        .then(async (userCredential) => {
            // Signed up 
            const user = userCredential.user;
            const userMetadata = {
                uid: user.uid,
                name: name,
                email: user.email!,
                organizations: []
            }
            await this.initializeUser(userMetadata)
            return userMetadata;
            // ...
        })
        .catch((error) => {
            throw error.message;
        });
    }

    public static authenticate(email: string, password: string) {
        return signInWithEmailAndPassword(this.auth, email, password)
        .then(async (userCredential) => {
            // Signed in 
            const authUser = userCredential.user;
            const user = await FBStorage.getFSDoc(`users/${authUser.uid}`) as UserMetaData
            return user;
        })
        .catch((error) => {
            throw error.message;
        });
    }
}