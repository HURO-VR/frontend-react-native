import * as functions from "firebase-functions";
import { FieldValue, getFirestore } from "firebase-admin/firestore";
import { initializeApp, getApps } from "firebase-admin/app";

const initFirebase = () => {
    let fs
    if (getApps().length == 0) initializeApp();
    try {
        fs = getFirestore()
    } catch (e) {
        initializeApp();
        fs = getFirestore()
    }
    return fs
}


export const OrganizationMemberUpdate = functions.firestore.onDocumentWritten("organizations/{orgID}", (event) => {
    const before = event.data?.before.exists ? event.data.before.data() : null;
    const after = event.data?.after.exists ? event.data.after.data() : null;
    const params = event.params

    const fs = initFirebase()
    const batch = fs.batch()

    if ((before && after && before.members.length < after.members.length) || (!before && after.members.length > 0)) {
        let newMembers
        if (before) {
            newMembers = (after.members).filter((newMember) => {
                return (before.members).find(currMember => currMember == newMember) == undefined
            })
        }
        else {
            newMembers = after.members
        }
        
        newMembers.forEach((memberID) => {
            batch.update(fs.doc(`users/${memberID}`), {
                organizations: FieldValue.arrayUnion(params.orgID)
            })
        })
        batch.commit()
    }
})
