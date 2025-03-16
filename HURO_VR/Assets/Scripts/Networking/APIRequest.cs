using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;
using System.Net.Security;
using System.Net;
using System.Security.Cryptography.X509Certificates;

public class SendPostRequest
{
    static bool TrustAllCertificates(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
    {
        return true; // Trust all certificates
    }

    static private bool AcceptAllCertifications(object sender, X509Certificate certification, X509Chain chain, SslPolicyErrors sslPolicyErrors)
    {
        return true;
    }

    // Before making your HTTPS request

    public static IEnumerator SendPostRequestToServer()
    {
        ServicePointManager.ServerCertificateValidationCallback = AcceptAllCertifications;


        Debug.Log("In SendPostRequestToServer");
        string url = "https://10.37.201.213:5000/run"; // Replace with your server's IP address
        // Create the JSON data to send in the body
        string json = "{\"arg\":\"HelloFromUnityPOST\"}";

        // Create the UnityWebRequest for the POST method
        using (UnityWebRequest request = UnityWebRequest.Post(url, json, "application/json"))
        {
            request.method = UnityWebRequest.kHttpVerbPOST;
            byte[] jsonToSend = new UTF8Encoding().GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(jsonToSend);
            request.downloadHandler = new DownloadHandlerBuffer();
            Debug.Log("Before return");
            // Send the request
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Server Response: " + request.downloadHandler.text);
            }
            else
            {
                Debug.LogError("Request Failed: " + request.error);
            }
        }
    }
}
