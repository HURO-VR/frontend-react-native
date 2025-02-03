using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Database_Models
{
    public struct SimulationMetaData
    {
        public string name; // User defined name for the simulation
        public string ID; // Simulation ID. Aka the folder for sim in the storage bucket
        public string dateCreated; // ISO formatted Date. Ex: 2025-02-02T12:34:56.789Z
        public string algorithmName; // The algorithm used for the simulation
        public string environmentName; // Name of the training environment to load.
    }
}
