using System;
using System.Collections;
using System.Collections.Generic;
using Firebase.Extensions;
using Firebase.Storage;
using Firebase.Firestore;
using UnityEngine;
using UnityEngine.Networking;
using System.Threading.Tasks;

public class FileType
{
    private FileType(string value) { Value = value; }

    public string Value { get; private set; }

    public static FileType Algorithm { get { return new FileType("algorithms"); } }
    public static FileType Model { get { return new FileType("models"); } }
    public override string ToString()
    {
        return Value;
    }
}

public class Storage : MonoBehaviour
{

    string org_name = "TEST_ORG";
    static FirebaseStorage storage;
    static FirebaseFirestore firestore;


    private void Awake()
    {
        storage = FirebaseStorage.DefaultInstance;
        firestore = FirebaseFirestore.DefaultInstance;

    }



    // FUNCTIONAL
    IEnumerator GetRequest(Uri uri, Action<byte[]> OnDownload)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            string[] pages = uri.ToString().Split('/');
            int page = pages.Length - 1;

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(pages[page] + ": Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(pages[page] + ": HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
                    OnDownload(webRequest.downloadHandler.data);
                    break;
            }
        }
    }


    public void DownloadFile(string filename, FileType fileType, string simulationID, Action<byte[]> OnDownload)
    {
        string branch = "simulations";
        string path = $"organizations/{org_name}/{branch}/{simulationID}/{fileType}/{filename}";
        Debug.Log("Downloading from path " + path);
        StorageReference reference = storage.GetReference(path);

        // Fetch the download URL
        reference.GetDownloadUrlAsync().ContinueWithOnMainThread(async (_task) => {
            if (!_task.IsFaulted && !_task.IsCanceled)
            {
                Uri uri = await _task;
                StartCoroutine(GetRequest(uri, OnDownload));
            }
        });
    }

    public string SaveBytesToFile(byte[] data, string filename, string pathName = "/")
    {
        if (pathName == null || pathName == "")
        {
            pathName = "/";
        } else if (pathName[pathName.Length - 1] != '/') {
            pathName += "/";
        }
        string path = Application.persistentDataPath + $"{pathName}{filename}";
        System.IO.File.WriteAllBytes(path, data);
        return path;
    }



    public async Task<Database_Models.SimulationMetaData[]> GetAllSimulationBundles()
    {
        Database_Models.SimulationMetaData[] simulationMetaDatas = new Database_Models.SimulationMetaData[0];
        Query simulationQuery = firestore.Collection($"organizations/{org_name}/simulations");
        await simulationQuery.GetSnapshotAsync().ContinueWithOnMainThread( (task) => {
            QuerySnapshot snapshot = task.Result;
            simulationMetaDatas = new Database_Models.SimulationMetaData[snapshot.Count];
            Debug.Log($"Num meta data: {simulationMetaDatas.Length} == {snapshot.Count}");
            int count = 0;
            foreach (DocumentSnapshot documentSnapshot in snapshot.Documents)
            {
                
                Dictionary<string, object> simData = documentSnapshot.ToDictionary();
                foreach (KeyValuePair<string, object> pair in simData)
                {
                    switch (pair.Key)
                    {
                        case "algorithmName":
                            simulationMetaDatas[count].algorithmFilename = (string)pair.Value;
                            break;
                        case "environmentName":
                            simulationMetaDatas[count].environmentName = (string)pair.Value;
                            break;
                        case "name":
                            simulationMetaDatas[count].name = (string)pair.Value;
                            break;
                        case "ID":
                            simulationMetaDatas[count].ID = (string)pair.Value;
                            break;
                        case "dateCreated":
                            simulationMetaDatas[count].dateCreated = (string)pair.Value;
                            break;
                    }  
                }
                count++;
            };
        });
        return await Task.FromResult(simulationMetaDatas);
    }




    public void SetOrganization(string org)
    {
        org_name = org;
    } 


}
