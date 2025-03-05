using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class TelemetryRecorder : MonoBehaviour
{
    public bool recordEveryFrame = true; // Record data every frame
    public float recordInterval = 1f; // Time interval (in seconds) to record data (if not recording every frame)
    private float timer = 0f;
    private string filePath;

    private List<TelemetryData> telemetryData = new List<TelemetryData>();

    void Start()
    {
        // Set the file path for the telemetry data
        filePath = Path.Combine(Application.persistentDataPath, "telemetry.json");

        // Clear the file if it already exists
        if (File.Exists(filePath))
        {
            File.WriteAllText(filePath, string.Empty);
        }

        // Initialize the telemetry data list
        telemetryData = new List<TelemetryData>();
    }

    void Update()
    {
        if (recordEveryFrame)
        {
            RecordTelemetry();
        }
        else
        {
            // Update the timer for interval-based recording
            timer += Time.deltaTime;
            if (timer >= recordInterval)
            {
                RecordTelemetry();
                timer = 0f; // Reset the timer
            }
        }
    }

    void RecordTelemetry()
    {
        // Create a new telemetry entry
        TelemetryData entry = new TelemetryData
        {
            Timestamp = Time.time,
            Position = transform.position,
            Rotation = transform.rotation.eulerAngles,
            DeltaRotation = CalculateDeltaRotation(transform.rotation)
        };

        // Add to the list
        telemetryData.Add(entry);

        // Log the data for debugging
        Debug.Log($"Recorded: {JsonUtility.ToJson(entry)}");
    }

    private Vector3 CalculateDeltaRotation(Quaternion currentRotation)
    {
        // Calculate change in rotation (if needed)
        // You can compare with the previous frame's rotation
        return Vector3.zero; // Replace with actual calculation
    }

    public string GetTelemetryDataAsJson()
    {
        // Convert the list to JSON
        TelemetryDataWrapper wrapper = new TelemetryDataWrapper { Data = telemetryData };
        return JsonUtility.ToJson(wrapper, true);
    }

    public void SaveTelemetryDataToFile()
    {
        string jsonData = GetTelemetryDataAsJson();
        File.WriteAllText(filePath, jsonData);
        Debug.Log("Telemetry data saved to: " + filePath);
    }

    [System.Serializable]
    public class TelemetryData
    {
        public float Timestamp;
        public Vector3 Position;
        public Vector3 Rotation;
        public Vector3 DeltaRotation;
    }

    [System.Serializable]
    public class TelemetryDataWrapper
    {
        public List<TelemetryData> Data;
    }
}