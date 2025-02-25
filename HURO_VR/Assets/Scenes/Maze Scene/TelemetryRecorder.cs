using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class TelemetryRecorder : MonoBehaviour
{
    public float recordInterval = 1f; // Time interval (in seconds) to record data
    private float timer = 0f;
    private string filePath;

    void Start()
    {
        // Set the file path for the telemetry data
        filePath = Path.Combine(Application.dataPath, "Scenes/Maze Scene/TelemetryData.txt");

        // Clear the file if it already exists
        if (File.Exists(filePath))
        {
            File.WriteAllText(filePath, string.Empty);
        }

        // Write the header to the file
        string header = "Timestamp, Position (X, Y, Z), Rotation (X, Y, Z)";
        File.AppendAllText(filePath, header + Environment.NewLine);
    }

    void Update()
    {
        // Update the timer
        timer += Time.deltaTime;

        // Check if it's time to record data
        if (timer >= recordInterval)
        {
            RecordTelemetry();
            timer = 0f; // Reset the timer
        }
    }

    void RecordTelemetry()
    {
        // Get the current timestamp
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");

        // Get the position and rotation of the GameObject
        Vector3 position = transform.position;
        Vector3 rotation = transform.rotation.eulerAngles;

        // Format the data as a string
        string data = $"{timestamp}, {position.x:F2}, {position.y:F2}, {position.z:F2}, {rotation.x:F2}, {rotation.y:F2}, {rotation.z:F2}";

        // Output the data to the Unity Debug Log
        Debug.Log(data);

        // Append the data to the file
        File.AppendAllText(filePath, data + Environment.NewLine);
    }
}