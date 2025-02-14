import { SafeAreaView } from "react-native-safe-area-context"
import { CreateOrganizationForm } from "./components/CreateOrganizationForm"
import { useEffect, useState } from "react"
import { UserMetaData } from "@/firebase/models"
import { FBStorage } from "@/firebase/storage"
import { FBAuth } from "@/firebase/auth"
import { Redirect, useRouter } from "expo-router"
import { red } from "react-native-reanimated/lib/typescript/Colors"
import { styles } from "./styles/styles"


const CreateOrganization = () => {
    const [user, setUser] = useState(null as UserMetaData | null)
    const [redirect, setRedirect] = useState("" as "/login")
    const router = useRouter()
    
    useEffect(() => {
        if (FBAuth.isSignedIn()) FBStorage.getFSDoc(`users/${FBAuth.getUID()}`).then(setUser)
        else setRedirect("/login")
    }, [])

    return (
        redirect.length == 0 ?
        <SafeAreaView style={{...styles.container}}>
            {user && <CreateOrganizationForm 
                user={user} 
                initialVisibility={true}
                onSubmit={() => {
                    router.push("/home")
                }}
            />}
        </SafeAreaView> :
        <Redirect href={redirect} />
    )
}

export default CreateOrganization