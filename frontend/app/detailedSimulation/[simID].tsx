import React, { useEffect } from 'react';
import { View, Text, Image, StyleSheet, ScrollView } from 'react-native';
import { MaterialIcons } from '@expo/vector-icons';
import { SimulationMetaData, SimulationRun } from '@/firebase/models';
import { useLocalSearchParams } from 'expo-router';
import { FBStorage } from '@/firebase/storage';


const DetailedSimulation = () => {

  const { simID } = useLocalSearchParams();
  const [metadata, setMetadata] = React.useState<SimulationMetaData | null>(null);
  const [simRuns, setSimRuns] = React.useState<SimulationRun[]>([]);

  useEffect(() => {
    FBStorage.getSimulationMetaData("TEST_ORG", simID as string).then((data) => {
      setMetadata(data);
    });

    FBStorage.getSimulationRuns("TEST_ORG", simID as string).then((data) => {
      setSimRuns(data);
    });
  }, []);

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
    <View style={styles.container}>
      <View style={styles.header}>
        <Text style={styles.title}>{metadata?.name}</Text>
        <View style={styles.headerIcons}>
          <MaterialIcons name="content-copy" size={24} color="#666" />
          <MaterialIcons name="delete" size={24} color="#666" style={styles.iconSpacing} />
        </View>
      </View>

      <View style={styles.content}>
        {/* Left Panel */}
        <View style={styles.leftPanel}>
          <View style={styles.section}>
            <Text style={styles.sectionLabel}>Algorithm</Text>
            <Text style={styles.sectionValue}>{metadata?.algorithmFilename}</Text>
          </View>

          <View style={styles.section}>
            <Text style={styles.sectionLabel}>3D Model</Text>
            <Text style={styles.sectionValue}>Sphere.gbl</Text>
            <View style={styles.spherePreview}>
              <View style={styles.sphere} />
            </View>
          </View>

          <View style={styles.section}>
            <Text style={styles.sectionLabel}>3D Scene</Text>
            <View style={styles.grid}>
              <GridPoint color="red" />
              <GridPoint color="lightblue" />
              <GridPoint color="green" />
              <View style={styles.gridLabels}>
                <Text style={styles.gridLabel}>Robot Start     1 1 0</Text>
                <Text style={styles.gridLabel}>Position Goal  -1 -1 0</Text>
                <Text style={styles.gridLabel}>User Start      0 0 0</Text>
              </View>
            </View>
          </View>
        </View>

        {/* Middle Panel */}
        <View style={styles.middlePanel}>
          <Text style={styles.panelTitle}>Simulation Runs</Text>
          <ScrollView>
            {simRuns.map((run) => (
              <View key={run.runID} style={styles.runItem}>
                <View style={styles.runInfo}>
                  <RenderStatusIcon status={run.status} />
                  <Text style={styles.runText}>{run.name}</Text>
                </View>
                {run.starred ? (
                  <MaterialIcons name="star" size={20} color="#FFB800" />
                ) : (
                  <MaterialIcons name="star-border" size={20} color="#666" />
                )}
              </View>
            ))}
          </ScrollView>
        </View>

        {/* Right Panel */}
        <View style={styles.rightPanel}>
          <Text style={styles.panelTitle}>Simulation Data</Text>
          <Text style={styles.dataTitle}>Run #3</Text>
          <View style={styles.dataItem}>
            <Text>Collisions</Text>
            <Text style={styles.collisionCount}>9</Text>
          </View>
          <View style={styles.dataItem}>
            <Text>Deadlock</Text>
            <MaterialIcons name="check-circle" size={20} color="green" />
          </View>
          <View style={styles.dataItem}>
            <Text>Time to Complete</Text>
            <Text>1:30</Text>
          </View>
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
    margin: 10,
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
    flex: 1,
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
    marginBottom: 15,
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
    fontSize: 16,
    fontWeight: 'bold',
    marginBottom: 10,
  },
  dataItem: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    marginBottom: 10,
  },
  collisionCount: {
    color: 'red',
  },
});

export default DetailedSimulation;