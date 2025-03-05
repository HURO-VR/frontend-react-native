using UnityEngine;
using System;
using System.Collections;
using Renci.SshNet;
using Newtonsoft.Json;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

public class RemoteScriptExecutor : MonoBehaviour
{
    [Header("SSH Connection Settings")]
    [Tooltip("External IP address of the SSH server")]
    public string externalIp = "104.154.36.87";

    [Tooltip("SSH Username")]
    public string username = "michaelmarcotte";

    string privateKeyPath = Application.streamingAssetsPath + "/gcp-vm-key";

    string remoteScriptPath = "/home/accou/Python_Scripts/Python/main.py";

    [Header("Logging")]
    [Tooltip("Enable detailed logging")]
    public bool enableLogging = true;

    // Callback for when script execution is complete
    public delegate void ScriptExecutionCompleted(string result);
    public event ScriptExecutionCompleted OnScriptExecutionComplete;
    SshClient client;

    // Callback for logging
    private void Log(string message)
    {
        if (enableLogging)
        {
            Debug.Log($"[RemoteScriptExecutor] {message}");
        }
    }

    // Public method to trigger SSH script execution
    public void ExecuteRemoteScript(string json_param)
    {
        StartCoroutine(ExecuteRemoteScriptCoroutine(json_param));
    }

    public void OpenSSHConnection()
    {
        var privateKeyFile = new PrivateKeyFile(privateKeyPath);
        var authMethod = new PrivateKeyAuthenticationMethod(username, privateKeyFile);
        var connectionInfo = new ConnectionInfo(externalIp, username, authMethod);
        // Create SSH connection
        client = new SshClient(connectionInfo);
        // Connect to the SSH server
        client.Connect();
        Log("SSH connection established.");
    }

    public void CloseSSHConnection()
    {
        if (client != null)
        {
            client.Disconnect();
        }
        client = null;
    }

    public string ExecuteCommand(string param)
    {
        string commandToExecute = $"python3 {remoteScriptPath} '{param}'";
        using (var cmd = client.CreateCommand(commandToExecute))
        {
            cmd.Execute();

            if (cmd.ExitStatus == 0)
            {
                Log($"Command executed successfully: {commandToExecute}");
                string result = cmd.Result.Trim();
                OnScriptExecutionComplete?.Invoke(result); 
                return result;
            }
            else
            {
                Debug.LogError($"Command failed. Command: {commandToExecute}, Error: {cmd.Error}");
                return string.Empty;
            }
        }
    }

    private IEnumerator ExecuteRemoteScriptCoroutine(string param)
    {
        var timer = new Stopwatch();
        // Placeholder to allow for async operations if needed
        yield return null;
        // Load the private key
        var privateKeyFile = new PrivateKeyFile(privateKeyPath);
        var authMethod = new PrivateKeyAuthenticationMethod(username, privateKeyFile);
        var connectionInfo = new ConnectionInfo(externalIp, username, authMethod);


        // Create SSH connection
        using (var client = new SshClient(connectionInfo))
        {
            // Connect to the SSH server
            client.Connect();
            Log("SSH connection established.");
                
            // Prepare command to execute remote Python script
            string commandToExecute = $"python3 {remoteScriptPath} '{param}'";

            // Execute the remote script
            
            string result = ExecuteCommand(client, commandToExecute);

            // Log runtime
            var runtime = timer.Elapsed;
            Log($"Algo Runtime: {runtime} seconds");

            // Parse the result
            Log($"Script Output: {result}");

            // Invoke completion callback
            OnScriptExecutionComplete?.Invoke(result);
            // Disconnect
            client.Disconnect();
            Log("SSH connection closed.");
        }

    }

    // Helper method to execute SSH commands and return output
    private string ExecuteCommand(SshClient client, string command)
    {
        using (var cmd = client.CreateCommand(command))
        {
            cmd.Execute();

            if (cmd.ExitStatus == 0)
            {
                Log($"Command executed successfully: {command}");
                return cmd.Result.Trim();
            }
            else
            {
                Debug.LogError($"Command failed. Command: {command}, Error: {cmd.Error}");
                return string.Empty;
            }
        }
    }

    // Example usage method
    public void StartRemoteScriptExecution()
    {
        // Add a listener to handle the script execution result
        OnScriptExecutionComplete += HandleScriptExecutionResult;

        // Trigger the script execution
       // ExecuteRemoteScript("");
    }

    // Example result handler
    private void HandleScriptExecutionResult(string result)
    {
        if (result != null)
        {
            try
            {
                // Optionally parse the JSON result
                var parsedResult = JsonConvert.DeserializeObject<dynamic>(result);
                Debug.Log($"Parsed Result: {parsedResult}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to parse result: {ex.Message}");
            }
        }
        else
        {
            Debug.LogError("Script execution failed or returned null result.");
        }

        // Remove the listener to prevent multiple calls
        OnScriptExecutionComplete -= HandleScriptExecutionResult;
    }
}