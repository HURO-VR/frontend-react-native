import { StatusBar } from "react-native";

export interface SimulationMetaData {
    name: string; // User defined name for the simulation
    ID: string; // Simulation ID. Aka the folder for sim in the storage bucket
    dateCreated: string; // ISO formatted Date. Ex: 2025-02-02T12:34:56.789Z
    algorithmFilename: string; // The algorithm used for the simulation
    environmentName: string; // The environment used for the simulation. Stored in "Resources/Training_Environments" in Unity
    runs: number; // Number of times the simulation has been run.
    modelFilename?: string; // Model Filename
}

export interface Organization {
    id: string
    simulations: string[] // Array of SimIDs
    members: string[] // User IDs
    admins: string[] // User IDs
    name: string,
    dateCreated: string
}

export enum RunStatus {
    failed = "failed",
    warning = "warning",
    success = "success"
}

export interface XYZ {
    x: number,
    y: number,
    z: number
}

export interface RobotData {
    robotStart: XYZ, // Robot starting position
    robotEnd: XYZ, // Robot ending position
    robotPath: XYZ[], // Array of robot path points
    name: string, // Robot ID
    timeToComplete: number, // Time in milliseconds
    collisions: XYZ[], // Array of collision points
    goalPosition: XYZ, // Goal position
    goalReached: boolean, // True if the robot reached the goal
}

export interface ObstacleData {
    position: XYZ, // Obstacle position
    radius: number, // Obstacle size
};

export interface SimulationRunData {
    timeToComplete: number, // Time in milliseconds
    totalCollisions: XYZ[], // Array of collision points
    deadlock: boolean, // True if the simulation ended in a deadlock
    robotData: RobotData[], // Array of robot IDs
    obstacleData: ObstacleData[], // Array of obstacle data
}
export interface SimulationRun {
    uid: string,
    dateCreated: string,
    status: RunStatus,
    simID: string,
    starred: boolean,
    runID: string,
    name: string,
    data: SimulationRunData
    serverHits: number,
    errorMessage: string
}

export interface UserMetaData {
    uid: string // User unique identifier. Equal to auth id
    name: string
    email: string
    organizations: string[] // OrgIDs
}

export enum EnvironmentTypes {
    mixedReality = "mixed-reality",
    emptyRoom = "empty-room",
    // Add more environments here
}

export const EnvImages: { [key: string]: any } = {
    [EnvironmentTypes.emptyRoom]: require(`../assets/images/Training_Envs/empty-room.jpg`),
    // Add more environments here
};

export enum FileUploadType {
    algorithm = 'algorithms',
    model = 'models',
}

export namespace AcceptedFileTypes {
    export enum Algorithm {
        onnx = '.onnx',
        py = '.py'
    }

    export enum Model {
        glb = '.glb'
    }

    export function checkFilename(filename: string, type: FileUploadType): boolean {
        switch (type) {
            case FileUploadType.algorithm:
                for (let item in AcceptedFileTypes.Algorithm) {
                    console.log(item)
                    if (filename.endsWith(item)) return true;
                }
            case FileUploadType.model:
                for (let item in AcceptedFileTypes.Model) {
                    if (filename.endsWith(item)) return true;
                }
            default:
                return false;
        }
    }

}

