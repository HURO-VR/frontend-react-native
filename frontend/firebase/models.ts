export interface SimulationMetaData {
    name: string; // User defined name for the simulation
    ID: string; // Simulation ID. Aka the folder for sim in the storage bucket
    dateCreated: string; // ISO formatted Date. Ex: 2025-02-02T12:34:56.789Z
    algorithmFilename: string; // The algorithm used for the simulation
    environmentName: string; // The environment used for the simulation. Stored in "Resources/Training_Environments" in Unity
}

export enum EnvironmentTypes {
    emptyRoom = "empty-room",
    // Add more environments here
}

