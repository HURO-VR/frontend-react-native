using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SessionController : MonoBehaviour
{
    public Database_Models.SimulationMetaData[] simulationMetaData = null;
    string Organization_ID = DemoCredentials.organizationID;
    Database_Models.SimulationMetaData selectedSimulation;
    [SerializeField] Transform simulationsMenuParent;
    [SerializeField] GameObject simulationItem;
    [SerializeField] TMP_Dropdown simulationDropdown;

    Storage db;
    // Start is called before the first frame update
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

        });
    }

    void SetItemName(GameObject item, string name)
    {
        string text = item.GetComponent<TextMeshProUGUI>()?.text;
        if (text != null) text = name;
    }

    public void SetSelectedSimulation(int index)
    {
        if (index < simulationMetaData?.Length) selectedSimulation = simulationMetaData[index];
        else Debug.LogWarning($"HURO: Simulation out of range of {simulationMetaData.Length} simulations");
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
