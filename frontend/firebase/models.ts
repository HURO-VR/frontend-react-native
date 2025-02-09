
export interface SimulationMetaData {
    name: string; // User defined name for the simulation
    ID: string; // Simulation ID. Aka the folder for sim in the storage bucket
    dateCreated: string; // ISO formatted Date. Ex: 2025-02-02T12:34:56.789Z
    algorithmFilename: string; // The algorithm used for the simulation
    environmentName: string; // The environment used for the simulation. Stored in "Resources/Training_Environments" in Unity
    runs: number; // Number of times the simulation has been run.
}

export enum EnvironmentTypes {
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
        onnx = '.onnx'
    }

    export enum Model {
        glb = '.glb'
    }

    export function checkFilename(filename: string, type: FileUploadType): boolean {
        switch (type) {
            case FileUploadType.algorithm:
                for (let item in AcceptedFileTypes.Algorithm) {
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

