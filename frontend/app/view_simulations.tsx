import { Button, ListRenderItem, StyleSheet, Text, View } from "react-native";
import FileUpload from "./components/fileUpload";
import { useEffect, useState } from "react";
import { DefaultStyles } from "./styles/DefaultStyles";
import { Link, useRouter } from "expo-router";
import TextStyles from "./styles/textStyles";
import { FlatList } from "react-native-gesture-handler";
import { SimulationMetaData } from "@/firebase/models";
import { SimulationListEntry } from "./components/simulationListEntry";
import { FBStorage } from "@/firebase/storage";

export default function ViewSimulations() {
  const [allSimulations, setAllSimulations] = useState<SimulationMetaData[]>([]);

  useEffect(() => {
    // Fetch all simulations from Firebase
    FBStorage.getAllSimulationsMetaData("TEST_ORG").then((sims) => {
        setAllSimulations(sims);
    });
  }, []);

  const router = useRouter();

  return (
    <View
      style={{
        flex: 1,
        justifyContent: "center",
        alignItems: "center",
      }}
    >
        <View style={{ marginVertical: 10 }} />
        <Text style={TextStyles.h3}>Created Simulations: </Text>
        {allSimulations.length > 0 ? <FlatList 
            data={allSimulations} 
            renderItem={({item}) => {
                return (<SimulationListEntry
                    title={item.name}
                    algorithmName={item.algorithmFilename}
                    dateCreated={item.dateCreated.split("T")[0]}
                    onPress={() => {
                      router.push(`/detailed_simulation`);
                    }}
                />);
            }}        
        /> :
        <View>
            <Text style={{marginBottom: 10, ...TextStyles.subtitle}}>No simulations found.</Text>
            <Link href="/create_simulation"
            style={{
              color: 'blue',
              textDecorationLine: 'underline',
              ...TextStyles.h6
             }}>Create Simulation</Link>
        </View>
        }
        
      
    </View>
  );
}
