import * as functions from "firebase-functions";
import { FieldValue, getFirestore, initializeFirestore } from "firebase-admin/firestore";
import { initializeApp } from "firebase-admin/app";

export const OrganizationMemberUpdate = functions.firestore.onDocumentWritten("organizations/{orgID}", (event) => {
    const before = event.data?.before.exists ? event.data.before.data() : null;
    const after = event.data?.after.exists ? event.data.after.data() : null;
    const params = event.param
    initializeApp()
    const fs = initializeFirestore()
    const batch = fs.batch()

    if ((before && after && before.members.length < after.members.length) || after.members.length > 1) {
        const newMembers = (after.members).filter((newMember) => {
            return (before.members).find(currMember => currMember == newMember) == undefined
        })
        
        
        newMembers.forEach((memberID) => {
            batch.update(fs.doc(`users/${memberID}`), {
                organizations: FieldValue.arrayUnion(params.orgID)
            })
        })
        batch.commit()
    }
})
