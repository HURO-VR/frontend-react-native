import { FBAuth } from '@/firebase/auth';
import { Redirect, useRouter } from 'expo-router';
import { useEffect, useState } from 'react';
import {
  View,
  Text,
  TextInput,
  TouchableOpacity,
  StyleSheet,
  SafeAreaView,
} from 'react-native';
import UserSelector from './UserSelector';
import { styles } from '../styles/styles';
import { Organization, UserMetaData } from '@/firebase/models';
import { FBStorage } from '@/firebase/storage';
import { isLoading } from 'expo-font';
import ConditionalView from './ConditionalView';
import TextStyles from '../styles/textStyles';


interface Props {
    user: UserMetaData
    onSubmit: (org: Organization) => void
}

export const CreateOrganizationForm = ({ user, onSubmit }: Props) => {

    const [name, setName] = useState("")
    const [nameError, setNameError] = useState("")
    const [users, setUsers] = useState([] as UserMetaData[])
    const [selectedUsers, setSelectedUsers] = useState([] as UserMetaData[])
    const [membersError, setMembersError] = useState("")
    const [loading, setLoading] = useState(false)
    const [allOrgs, setAllOrgs] = useState([] as Organization[])
    const router = useRouter()

    const verifyOrg = () => {
        if (name.length < 3) {
            setNameError("Name must be at least 3 characters.")
            return false
        } if (allOrgs.find(org => org.name == name)) {
            setNameError("An organization already exists with this name.")
            return false
        }
        return true
    }

    const createOrganization = () => {
        setLoading(true)
        if (verifyOrg()) {
            FBStorage.createOrganization(name, user.uid, [...(selectedUsers.map((u) => u.uid)), user.uid]).then(onSubmit).finally(() => setLoading(false))
        } else setLoading(false)
    }

    useEffect(() => {
        FBStorage.getCollection("users").then((data) => {
            setUsers(data.filter((u) => u.uid != user.uid)) // Remove Self from list
        })
        FBStorage.getCollection("organizations").then((orgs) => {
            setAllOrgs(orgs)
        })
    }, [])

    return (
        <View style={{...styles.container}}>
            <View style={{width: "50%"}}>
            <Text style={{...TextStyles.h1, marginVertical: 20, color: "white", alignSelf: "flex-start"}}>Assemble your team.</Text>
                {/* Organization Name */}
                <View style={styles.inputContainer}>
                        <Text style={styles.label}>Organization Name</Text>
                        <TextInput
                            style={styles.input}
                            placeholder="Name"
                            placeholderTextColor="#666"
                            value={name}
                            onChangeText={setName}
                        />
                        <Text style={styles.errorText}>{nameError}</Text>
                </View>

                {/* Select Members */}
                <View style={styles.inputContainer}>
                    <Text style={styles.label}>Invite Members</Text>
                    <UserSelector users={users} selectedUsers={selectedUsers} setSelectedUsers={setSelectedUsers}/>
                    <Text style={styles.errorText}>{membersError}</Text>
                </View>
            </View>

            {/* Create Org Button */}
            <TouchableOpacity 
                style={{...styles.addButton, marginTop: 20, alignSelf: "center"}} 
                onPress={() => {
                    createOrganization()
                }}
                disabled={loading}
            >
              <Text style={styles.addButtonText}>Create New Organization</Text>
            </TouchableOpacity>

            <TouchableOpacity 
                style={{marginTop: 20, alignSelf: "center"}} 
                onPress={() => {
                    router.back()
                }}
                disabled={loading}
            >
              <Text style={{color: "white"}}>Back</Text>
            </TouchableOpacity>
        </View>

        // Add scroll view on login
        // Check first user experience.

    )
}

