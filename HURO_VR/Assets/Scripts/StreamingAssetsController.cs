using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using System.Collections.Generic;

public class StreamingAssetsToPersistent : MonoBehaviour
{
    [Header("List functions to call when loading is finished")]
    public UnityEvent OnLoadingFinished;
    bool finishedLoading = false;

    private void Start()
    {
        StartCoroutine(CopyAllFiles());
    }

    private IEnumerator CopyAllFiles()
    {
        string sourceRoot = Application.streamingAssetsPath;
        string destinationRoot = Application.persistentDataPath;

        List<string> filesToCopy = new List<string>();

        // Get all files recursively (Desktop & iOS allow Directory.GetFiles, but Android requires a workaround)
        if (Application.platform == RuntimePlatform.Android)
        {
            // Manually list files for Android or store a pre-generated file list in StreamingAssets
            Debug.LogWarning("On Android, you may need to store a file list manually in StreamingAssets.");
        }
        else
        {
            string[] filePaths = Directory.GetFiles(sourceRoot, "*", SearchOption.AllDirectories);
            foreach (string fullPath in filePaths)
            {
                string relativePath = fullPath.Substring(sourceRoot.Length + 1).Replace("\\", "/");
                filesToCopy.Add(relativePath);
            }
        }
        int copied = 0;
        foreach (string relativePath in filesToCopy)
        {
            string sourcePath = Path.Combine(sourceRoot, relativePath);
            string destinationPath = Path.Combine(destinationRoot, relativePath);

            string destinationDir = Path.GetDirectoryName(destinationPath);
            if (!Directory.Exists(destinationDir))
            {
                Directory.CreateDirectory(destinationDir);
            }

            if (!File.Exists(destinationPath)) // Avoid overwriting existing files
            {
                using (UnityWebRequest request = UnityWebRequest.Get(sourcePath))
                {
                    yield return request.SendWebRequest();

                    if (request.result == UnityWebRequest.Result.Success)
                    {
                        if (request.downloadHandler.data != null) File.WriteAllBytes(destinationPath, request.downloadHandler.data);
                        copied++;
                        Debug.Log($"Copied {copied / filesToCopy.Count * 100}%");
                    }
                    else
                    {
                        Debug.LogError($"Failed to copy {relativePath}: {request.error}");
                    }
                }
            }
        }

        OnLoadingFinished?.Invoke();
        finishedLoading = true;
        Debug.Log("Complete file transfer");
    }

    public bool IsFinishedLoading()
    {
        return finishedLoading;
    }
}
