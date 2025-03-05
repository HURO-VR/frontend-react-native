import { View, Text, TouchableOpacity } from "react-native";
import TextStyles from "../styles/textStyles";

interface SimulationListEntryProps {
    title: string;
    algorithmName: string;
    dateCreated: string;
    onPress?: () => void;
}

export const SimulationListEntry = ({ title, algorithmName, dateCreated, onPress }: SimulationListEntryProps) => {
    return (
        <TouchableOpacity style={{ padding: 20 }} onPress={() => {
            if (onPress) onPress();
        }}>
            <Text style={TextStyles.subtitle}>Simulation Name:</Text>
            <Text style={{
                fontWeight: "bold",
                marginBottom: 10,
            }}>{title}</Text>

            <Text style={{fontStyle: "italic"}}>Algorithm Name:</Text>
            <Text>{algorithmName}</Text>
            <View style={{marginVertical: 2.5}} />
            <Text style={{fontStyle: "italic"}}>Date Created:</Text>
            <Text>{dateCreated}</Text>
        </TouchableOpacity>
    );
};