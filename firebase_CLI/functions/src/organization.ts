import * as functions from "firebase-functions";

export const OrganizationMemberUpdate = functions.firestore.onDocumentWritten("organizations/{orgID}", (event) => {
    const before = event.data?.before.exists ? event.data.before.data() : null;
    const after = event.data?.after.exists ? event.data.after.data() : null;
    
    if (before && after && before.members.length < after.members.length) {
        const newMembers = after.members.filter()
    }
})
