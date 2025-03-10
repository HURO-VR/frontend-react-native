import React, { useEffect } from 'react';
import { View, Text, Image, StyleSheet, ScrollView, ViewStyle, StyleProp, TouchableOpacity } from 'react-native';
import { MaterialIcons } from '@expo/vector-icons';
import { RunStatus, SimulationMetaData, SimulationRun } from '@/firebase/models';
import { useLocalSearchParams } from 'expo-router';
import { FBStorage } from '@/firebase/storage';
import RobotPathMap from '../components/RobotPathMap';
import SceneVisualization from '../components/SceneVisualization';

interface Props {
  metadata: SimulationMetaData
  viewStyle?: StyleProp<ViewStyle>
  simID:string
  orgID:string
}
const DetailedSimulation = ({metadata, viewStyle, simID, orgID}: Props) => {

  const [simRuns, setSimRuns] = React.useState<SimulationRun[]>([]);
  const [selectedRun, setSelectedRun] = React.useState<SimulationRun | null>(null);

  const loadSimRuns = () => {
    FBStorage.getCollection(`organizations/${orgID}/simulations/${simID}/runs`).then((data) => {
      setSimRuns(data);
      if (data.length > 0) setSelectedRun(data[0]);
      FBStorage.subscribeToCollection(`organizations/${orgID}/simulations/${simID}/runs`, (data) => setSimRuns(data as SimulationRun[]));
    });
  }
  useEffect(() => {
    loadSimRuns();
  }, []);

  useEffect(() => {
    setSimRuns([]);
    setSelectedRun(null);
    loadSimRuns();
}, [simID])

  const RenderStatusIcon = ({ status }: any) => {
    switch (status) {
      case 'failed':
        return <MaterialIcons name="close" size={20} color="red" />;
      case 'warning':
        return <MaterialIcons name="warning" size={20} color="#FFB800" />;
      case 'success':
        return <MaterialIcons name="check-circle" size={20} color="green" />;
      default:
        return null;
    }
  };

  const GridPoint = ({ color }: any) => (
    <View style={[styles.gridPoint, { backgroundColor: color }]} />
  );

  return (
    <View style={[styles.container, viewStyle]}>
      <View style={styles.header}>
        <Text style={styles.title}>{metadata.name}</Text>
        <View style={styles.headerIcons}>
          <MaterialIcons name="content-copy" size={24} color="#666" />
          <MaterialIcons name="delete" size={24} color="#666" style={styles.iconSpacing} />
        </View>
      </View>

      <View style={styles.content}>

        {/* Simulation List Panel */}
        <View style={styles.middlePanel}>
          <Text style={styles.panelTitle}>Simulation Runs</Text>
          {simRuns.length > 0  ? <ScrollView>
            {simRuns.map((run) => (
              <TouchableOpacity key={run.runID} style={styles.runItem} onPress={() => {
                setSelectedRun(run)
              }}>
                <View style={styles.runInfo}>
                  <RenderStatusIcon status={run.status} />
                  <Text style={styles.runText}>{run.name}</Text>
                  {run.errorMessage && <Text style={{marginLeft: 20}}>{run.errorMessage.substring(0, 8)}</Text>}
                </View>
                <TouchableOpacity onPress={() => {
                  FBStorage.updateDoc(run.runID, `organizations/${orgID}/simulations/${simID}/runs`, { starred: !run.starred });
                  run.starred = !run.starred;
                  setSimRuns([...simRuns]);
                }}>
                  {run.starred ? (
                    <MaterialIcons name="star" size={20} color="#FFB800" />
                  ) : (
                    <MaterialIcons name="star-border" size={20} color="#666" />
                  )}
                </TouchableOpacity>
              </TouchableOpacity>
            ))}
          </ScrollView> : <Text>No Runs</Text>}
        </View>

        {/* Data Panel */}
        <View style={styles.leftPanel}>
        <Text style={styles.panelTitle}>Simulation Data</Text>
        {selectedRun && <>
        
          <View>
          <View style={styles.dataItem}>
              <Text style={styles.dataTitle}>Date:</Text>
              <Text>{selectedRun.dateCreated.substring(0, 19)}</Text>
            </View>
            {selectedRun.status != RunStatus.warning ? <>
            <View style={styles.dataItem}>
              <Text style={styles.dataTitle}>Collisions</Text>
              <Text style={[styles.collisionCount, {color: selectedRun.data.totalCollisions.length == 0 ? "green" : "red"}]}>{selectedRun.data.totalCollisions.length}</Text>
            </View>
            <View style={styles.dataItem}>
              <Text style={styles.dataTitle}>Deadlock</Text>
              {!selectedRun?.data.deadlock ? <MaterialIcons name="check-circle" size={20} color="green" /> : <MaterialIcons name="error" size={20} color="red" />}
            </View>
            <View style={styles.dataItem}>
              <Text style={styles.dataTitle}>Time to Complete</Text>
              <Text>{selectedRun?.data.timeToComplete/1000} seconds</Text>
            </View>
            </> : <Text>{selectedRun.errorMessage}</Text>}
          </View>
          
          <View style={styles.section}>
            <Text style={styles.panelTitle}>Scene Data</Text>
            <View style={styles.grid}>
              <SceneVisualization simulationRun={selectedRun}/>
            </View>
          </View>
          <View style={styles.section}>
            <Text style={styles.panelTitle}>Environment Setup</Text>
            <Text style={styles.sectionValue}>{metadata?.environmentName}</Text>
          </View>

          <View style={styles.section}>
            <Text style={styles.sectionLabel}>3D Model</Text>
            <Text style={styles.sectionValue}>{metadata.modelFilename ? metadata.modelFilename : "Default"}</Text>
          </View>
            </>}
        </View>
      </View>
    </View>
  );
};

const styles = StyleSheet.create({
  container: {
    backgroundColor: '#fff',
    borderRadius: 10,
    padding: 20,
    width: "100%"
  },
  header: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    marginBottom: 20,
  },
  title: {
    fontSize: 24,
    fontWeight: 'bold',
  },
  headerIcons: {
    flexDirection: 'row',
  },
  iconSpacing: {
    marginLeft: 10,
  },
  content: {
    flexDirection: 'row',
    gap: 20,
  },
  leftPanel: {
    flex: 3,
    backgroundColor: '#f5f5f5',
    padding: 15,
    borderRadius: 8,
  },
  middlePanel: {
    flex: 1,
    backgroundColor: '#f5f5f5',
    padding: 15,
    borderRadius: 8,
  },
  rightPanel: {
    flex: 1,
    backgroundColor: '#f5f5f5',
    padding: 15,
    borderRadius: 8,
  },
  section: {
    marginBottom: 20,
  },
  sectionLabel: {
    fontWeight: 'bold',
    marginBottom: 5,
  },
  sectionValue: {
    color: '#666',
  },
  spherePreview: {
    alignItems: 'center',
    marginTop: 10,
  },
  sphere: {
    width: 40,
    height: 40,
    borderRadius: 20,
    backgroundColor: 'orange',
  },
  grid: {
    flexDirection: 'row',
    alignItems: 'center',
    marginTop: 10,
  },
  gridPoint: {
    width: 10,
    height: 10,
    borderRadius: 5,
    marginRight: 5,
  },
  gridLabels: {
    marginLeft: 10,
  },
  gridLabel: {
    fontFamily: 'monospace',
    fontSize: 12,
    marginBottom: 2,
  },
  panelTitle: {
    fontSize: 18,
    fontWeight: 'bold',
    marginBottom: 8,
  },
  runItem: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    paddingVertical: 10,
    paddingHorizontal: 15,
    backgroundColor: '#fff',
    marginBottom: 5,
    borderRadius: 5,
  },
  runInfo: {
    flexDirection: 'row',
    alignItems: 'center',
  },
  runText: {
    marginLeft: 10,
  },
  dataTitle: {
    marginRight: 15
  },
  dataItem: {
    flexDirection: 'row',
    alignItems: 'center',
    marginBottom: 10,
  },
  collisionCount: {
    color: 'red',
    fontWeight: 'bold',
  },
});

export default DetailedSimulation;