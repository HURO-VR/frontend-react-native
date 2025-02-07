using UnityEngine;
using Firebase.Database;
using System.Collections;

public class TelemetryTracker : MonoBehaviour
{
    private DatabaseReference dbReference;
    private string objectID = "TrackedObject_1"; // Unique ID for the object

    void Start()
    {
        // Initialize Firebase Database
        dbReference = FirebaseDatabase.DefaultInstance.RootReference;
        StartCoroutine(SendTelemetryData());
    }

    IEnumerator SendTelemetryData()
    {
        while (true)
        {
            // Get current position and rotation
            Vector3 position = transform.position;
            Quaternion rotation = transform.rotation;

            // Create telemetry data object
            var telemetryData = new
            {
                posX = position.x,
                posY = position.y,
                posZ = position.z,
                rotX = rotation.eulerAngles.x,
                rotY = rotation.eulerAngles.y,
                rotZ = rotation.eulerAngles.z,
                timestamp = System.DateTime.UtcNow.ToString("o") // ISO 8601 timestamp
            };

            // Convert to JSON and send to Firebase
            string json = JsonUtility.ToJson(telemetryData);
            dbReference.Child("telemetry").Child(objectID).SetRawJsonValueAsync(json);

            yield return new WaitForSeconds(0.5f); // Send data every 0.5 seconds
        }
    }
}
