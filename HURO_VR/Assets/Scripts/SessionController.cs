using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] Transform simulationsMenuParent;
    [SerializeField] GameObject simulationItem;
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
            foreach (var item in simulationMetaData)
            {
                GameObject entry = Instantiate(simulationItem);
                SetItemName(entry, item.name);
                entry.transform.SetParent(simulationsMenuParent);
                options.Add(item.name);
            }
            simulationDropdown.ClearOptions();
            simulationDropdown.AddOptions(options);
            if (selectedSimulation.ID == null || selectedSimulation.ID.Length == 0) SetSelectedSimulation(0);

            Debug.Log($"HURO: Loaded {data?.Length} bundles into menu.");

        });
    }

    void SetItemName(GameObject item, string name)
    {
        string text = item.GetComponent<TextMeshProUGUI>()?.text;
        if (text != null) text = name;
    }

    public void SetSelectedSimulation(int index)
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
        object json = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(data));
        db.UploadMetadata($"organizations/{Organization_ID}/simulations/{selectedSimulation.ID}/runs/{data.runID}", json);
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
