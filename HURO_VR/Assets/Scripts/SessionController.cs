using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Meta.XR.MRUtilityKit;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;

public class SessionController : MonoBehaviour
{
    public Database_Models.SimulationMetaData[] simulationMetaData = null;
    string Organization_ID = DemoCredentials.organizationID;
    string userID = DemoCredentials.nikitaID;
    Database_Models.SimulationMetaData selectedSimulation;
    int numberRuns = 1;    
    [SerializeField] TMP_Dropdown simulationDropdown;
    Storage db;

    void Awake()
    {
        db = FindAnyObjectByType<Storage>();
        db.SetOrganization(Organization_ID);
    }

    private void Start()
    {
        _ = db.GetAllSimulationBundles((data) =>
        {
            simulationMetaData = data;
            List<string> options = new List<string>();
            var reversed = simulationMetaData.Reverse(); // Earliest Simulation at top of list.
            foreach (var item in reversed)
            {
                options.Add(item.name);
            }
            simulationDropdown.ClearOptions();
            simulationDropdown.AddOptions(options);
            if (selectedSimulation.ID == null || selectedSimulation.ID.Length == 0) SetSelectedSimulation(0);

            Debug.Log($"HURO: Loaded {data?.Length} bundles into menu.");

        });
    }

    public void SetSelectedSimulation(int index = 0)
    {
        Debug.Log($"HURO: Selecting {index} simulation");
        if (index < simulationMetaData?.Length) { 
            selectedSimulation = simulationMetaData[index];
            db.GetFirestoreCollection($"organizations/{Organization_ID}/simulations/{selectedSimulation.ID}/runs", (data) =>
            {
                numberRuns = data.Count;
            });
        }
        else Debug.LogWarning($"HURO: Simulation {index} out of range of {simulationMetaData.Length} simulations");
    }

    public void UploadSimulationRunData(SimulationRun data)
    {
        data.uid = userID;
        data.simID = selectedSimulation.ID;
        data.name = $"Run {numberRuns + 1}";
        data.runID = data.name + "_" + data.runID;
        db.UploadMetadata($"organizations/{Organization_ID}/simulations/{selectedSimulation.ID}/runs/{data.runID}", data.ToDictionary());
        Debug.Log("Uploaded data collection");
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
