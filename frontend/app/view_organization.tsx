import { SimulationMetaData } from '@/firebase/models';
import { FBStorage } from '@/firebase/storage';
import React, { useEffect, useState } from 'react';
import {
  View,
  Text,
  StyleSheet,
  ScrollView,
  TouchableOpacity,
  SafeAreaView,
} from 'react-native';

const SimulationItem = ({ name, algorithm, runs }: any) => (
  <View style={styles.listItem}>
    <Text style={styles.itemName}>{name}</Text>
    <Text style={styles.itemAlgorithm}>{algorithm}</Text>
    <Text style={styles.itemRuns}>{runs}</Text>
  </View>
);

const TeamMemberItem = ({ name, runs }: any) => (
  <View style={styles.listItem}>
    <Text style={styles.itemName}>{name}</Text>
    <Text style={styles.itemRuns}>{runs}</Text>
  </View>
);

const OrganizationView = () => {
  const _simulations = [
    { name: 'Empty Room', algorithmFilename: 'Navigation', runs: 12 },
    { name: 'Corridor', algorithmFilename: 'Navigation', runs: 21 },
    { name: 'Crowd', algorithmFilename: 'Navigation', runs: 20 },
    { name: 'Bedroom', algorithmFilename: 'Grab + Move', runs: 54 },
    { name: 'Intersection', algorithmFilename: 'Navigation', runs: 26 },
  ];

  const teamMembers = [
    { name: 'Vencent Vang', runs: 12 },
    { name: 'Karan Soin', runs: 21 },
    { name: 'Michael Marcotte', runs: 20 },
    { name: 'Tasha Kim', runs: 54 },
    { name: 'Nikita X-One', runs: 26 },
  ];

  const [orgName, setOrgName] = useState("TEST_ORG")
  const [simulations, setSimulations] = useState([] as SimulationMetaData[])

  useEffect(() => {
    FBStorage.getAllSimulationsMetaData(orgName).then((data) => {
        setSimulations(data)
    })
  })

  return (
    <SafeAreaView style={styles.container}>
      <View style={styles.header}>
        <Text style={styles.headerText}>{orgName}</Text>
      </View>

      <ScrollView style={styles.content}>
        <View style={{flexDirection: "row", flex: 1}}>

            {/* First Section */}
            <View style={styles.section}>
            <View style={styles.sectionHeader}>
                <Text style={styles.sectionTitle}>Simulations</Text>
            </View>
            
            <View style={styles.listHeader}>
                <Text style={styles.columnHeader}>Name</Text>
                <Text style={styles.columnHeader}>Algorithm</Text>
                <Text style={styles.columnHeader}># Runs</Text>
            </View>
            
            {_simulations.map((sim, index) => (
                <SimulationItem
                key={index}
                name={sim.name}
                algorithm={sim.algorithmFilename}
                runs={sim.runs}
                />
            ))}
            
            <TouchableOpacity style={styles.addButton}>
                <Text style={styles.addButtonText}>New +</Text>
            </TouchableOpacity>
            </View>
            <View style={{margin: 10}} />
            {/* Second Section */}
            <View style={styles.section}>
            <View style={styles.sectionHeader}>
                <Text style={styles.sectionTitle}>Team Members</Text>
            </View>
            
            <View style={styles.listHeader}>
                <Text style={styles.columnHeader}>Name</Text>
                <Text style={styles.columnHeader}># Runs</Text>
            </View>
            
            {teamMembers.map((member, index) => (
                <TeamMemberItem
                key={index}
                name={member.name}
                runs={member.runs}
                />
            ))}
            
            <TouchableOpacity style={styles.addButton}>
                <Text style={styles.addButtonText}>Invite +</Text>
            </TouchableOpacity>
          </View>
        </View>
      </ScrollView>
    </SafeAreaView>
  );
};

const styles = StyleSheet.create({
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