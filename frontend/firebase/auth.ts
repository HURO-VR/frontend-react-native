import { initializeApp } from "firebase/app";
import { firebaseConfig } from "./config";
import { createUserWithEmailAndPassword, getAuth, signInWithEmailAndPassword } from "firebase/auth";
import {User} from "./models"
import { FBStorage } from "./storage";

export class FBAuth {
    static app = initializeApp(firebaseConfig);
    static auth = getAuth()

    public static isSignedIn(): boolean {
        if (this.auth.currentUser == null) {
            return false;
        }
        return true;
    }

    static initializeUser(user: User) {
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
            const user = await FBStorage.getDoc(`users/${authUser.uid}`) as User
            return user;
        })
        .catch((error) => {
            throw error.message;
        });
    }
}