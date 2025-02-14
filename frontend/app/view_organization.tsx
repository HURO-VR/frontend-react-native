import { Organization, SimulationMetaData, UserMetaData } from '@/firebase/models';
import { FBStorage } from '@/firebase/storage';
import { useLocalSearchParams, useRouter } from 'expo-router';
import { useEffect, useState } from 'react';
import {
  View,
  Text,
  StyleSheet,
  ScrollView,
  TouchableOpacity,
  SafeAreaView,
  TextInput,
} from 'react-native';
import ConditionalView from './components/ConditionalView';
import { CreateOrganizationForm } from './components/CreateOrganizationForm';
import UserSelector from './components/UserSelector';
import { styles } from './styles/styles';

const SimulationItem = ({ name, algorithm, runs }: any) => (
  <View style={_styles.listItem}>
    <Text style={_styles.itemName}>{name}</Text>
    <Text style={_styles.itemAlgorithm}>{algorithm}</Text>
    <Text style={_styles.itemRuns}>{runs}</Text>
  </View>
);

const TeamMemberItem = ({ name, runs }: any) => (
  <View style={_styles.listItem}>
    <Text style={_styles.itemName}>{name}</Text>
    <Text style={_styles.itemRuns}>{runs}</Text>
  </View>
);

const OrganizationView = () => {

  const router = useRouter()

  const [organization, setOrganization] = useState({
    name: "Create an Organization",
    simulations: [],
    members: [],
    admins: [],
    id: "",
    dateCreated: ""
  } as Organization)
  const [simulations, setSimulations] = useState([] as SimulationMetaData[])
  const [members, setMembers] = useState([] as UserMetaData[])

  const { param_user } = useLocalSearchParams();
  const user = JSON.parse(param_user as string) as UserMetaData;

  const [loading, setLoading] = useState(true)
  const [invitedUsers, setInvitedUsers] = useState([] as UserMetaData[])
  const [inviteAbleUsers, setInvitable] =  useState([] as UserMetaData[])
  const [inviteLoading, setInviteLoading] = useState(false)
  const [inviteErrorText, setInviteError] = useState("")
  const [isAdmin, setAdmin] = useState(false)


  useEffect(() => {
     if (user.organizations.length > 0) {
        FBStorage.getFSDoc(`organizations/${user.organizations[0]}`).then((org) => {
          setOrganization(org)
          setAdmin((org as Organization).admins.find((u) => u == user.uid) != undefined)
        })
     } else {
      setLoading(false)
     }
  }, [])

  useEffect(() => {
    if (organization.id.length > 0) {
      const promises = []
       promises.push(FBStorage.getAllSimulationsMetaData(organization.id).then((data) => {
          setSimulations(data)
        }))
        promises.push(FBStorage.getCollection(`users`).then((data) => {
          setMembers(data.filter((user) => organization.members.find(m => m == user.uid)))
          setInvitable(data.filter((user) => organization.members.find(m => m == user.uid) == undefined))
        }))
        Promise.all(promises).finally(() => {
          setLoading(false)
        })
  }
  }, [organization])

  return (
    <SafeAreaView style={_styles.container}>
      <View style={_styles.header}>
        <Text style={_styles.headerText}>{organization?.name}</Text>
      </View>

      {/* Org View */}
      {organization.id != "" && <ScrollView style={_styles.content}>
        <View style={{flexDirection: "row", flex: 1}}>
            {/* First Section */}
            <ConditionalView 
              style={_styles.section} 
              isVisible={organization.id != ""}
            >
              <View style={_styles.sectionHeader}>
                  <Text style={_styles.sectionTitle}>Simulations</Text>
              </View>
              
              <View style={_styles.listHeader}>
                  <Text style={_styles.columnHeader}>Name</Text>
                  <Text style={_styles.columnHeader}>Algorithm</Text>
                  <Text style={_styles.columnHeader}># Runs</Text>
              </View>
              
              {simulations.map((sim, index) => (
                  <SimulationItem
                  key={index}
                  name={sim.name}
                  algorithm={sim.algorithmFilename}
                  runs={sim.runs}
                  />
              ))}
              
              <TouchableOpacity style={_styles.addButton}
              onPress={() => {
                router.push("/create_simulation")
              }}>
                  <Text style={_styles.addButtonText}>New +</Text>
              </TouchableOpacity>
            </ConditionalView>

            <View style={{margin: 10}} />

            {/* Second Section */}
            <ConditionalView 
              style={_styles.section} 
              isVisible={organization.id != ""}
            >
              <View style={_styles.sectionHeader}>
                  <Text style={_styles.sectionTitle}>Team Members</Text>
              </View>
            
              <View style={_styles.listHeader}>
                  <Text style={_styles.columnHeader}>Name</Text>
                  <Text style={_styles.columnHeader}># Runs</Text>
              </View>
            
              {organization.id != "" && members.map((member, index) => (
                  <TeamMemberItem
                  key={index}
                  name={member.name}
                  //runs={member.runs}
                  />
              ))}
              <View style ={{marginTop: 30}}></View>
              {!loading && isAdmin && <>
              <UserSelector users={inviteAbleUsers} selectedUsers={invitedUsers} setSelectedUsers={setInvitedUsers}/>
              <Text style={styles.errorText}>{inviteErrorText}</Text>
              <TouchableOpacity style={_styles.addButton}
                onPress={() => {
                  if (invitedUsers.length == 0) setInviteError("Must invite at least one user.")
                  setInviteLoading(true)
                  FBStorage.addMembersToOrg(organization, invitedUsers.map(u => u.uid)).then(() => {
                    setInvitedUsers([])
                  }).catch(() => {
                    window.alert("Failed to add users")
                  }).finally(() => {
                    setInviteLoading(false)
                  })
                }}
                disabled={inviteLoading}>
          
                  <Text style={_styles.addButtonText}>Invite +</Text>
              </TouchableOpacity>
              </>}
          </ConditionalView>
        </View>
      </ScrollView>}


      {/* Create Organization Section*/}
      {organization.id == "" && !loading && <CreateOrganizationForm 
        user={user} 
        initialVisibility={organization.id == ""}
        onSubmit={(org) => {
          setOrganization(org)
        }}
      />}
    </SafeAreaView>
  );
};

const _styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: '#fff',
  },
  header: {
    padding: 16,
    backgroundColor: '#000',
  },
  headerText: {
    fontSize: 24,
    fontWeight: 'bold',
    color: '#fff',
  },
  content: {
    flex: 1,
    padding: 16,
  },
  section: {
    marginBottom: 24,
    backgroundColor: '#f5f5f5',
    borderRadius: 8,
    padding: 16,
    flex: 1
  },
  sectionHeader: {
    marginBottom: 16,
  },
  sectionTitle: {
    fontSize: 18,
    fontWeight: 'bold',
  },
  listHeader: {
    flexDirection: 'row',
    paddingVertical: 8,
    borderBottomWidth: 1,
    borderBottomColor: '#ddd',
  },
  columnHeader: {
    flex: 1,
    fontWeight: 'bold',
  },
  listItem: {
    flexDirection: 'row',
    paddingVertical: 12,
    borderBottomWidth: 1,
    borderBottomColor: '#eee',
  },
  itemName: {
    flex: 1,
  },
  itemAlgorithm: {
    flex: 1,
  },
  itemRuns: {
    flex: 1,
    textAlign: 'right',
  },
  addButton: {
    backgroundColor: '#00C853',
    padding: 8,
    borderRadius: 4,
    alignSelf: 'flex-start',
    marginTop: 16,
  },
  addButtonText: {
    color: '#fff',
    fontWeight: 'bold',
  },
});

export default OrganizationView;