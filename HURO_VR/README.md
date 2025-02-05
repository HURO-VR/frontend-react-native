# HURO Unity VR Environment

## Setup

1. Download [Firebase SDK](https://firebase.google.com/download/unity?hl=en&authuser=0&_gl=1*1hkvyve*_ga*MjczMTY1NDA4LjE3MzcyNTY4NDI.*_ga_CW55HF8NVT*MTczODE4NDEzMS41LjEuMTczODE4Nzk3Ni42LjAuMA..)

2. In your open Unity project, navigate to Assets > Import Package > Custom Package.

3. From the unzipped SDK, select to import the SDK for Firebase Storage.

4. In the Import Unity Package window, click Import.

5. Delete `Assets/ExternalDependencyManager/Editor/Google.IOSResolver.dll` and `Assets/ExternalDependencyManager/Editor/Google.IOSResolver.dll.mdb`

6. Locate `Assets/Firebase/Editor/Firebase.Editor.dll.meta` and uncheck `Validate References` in the inspector.


## Documentation

#### Downloading from Firebase Storage
`DownloadFile("filename", FileType, OnDownloaded);`

`filename` - Name of file in database, including extension (ex: .txt or .py)

`FileType` - Type of file to be retrieved. (ex: algorithm or model)

`OnDownloaded` - Callback that will be passed the data as a `byte[]`. Function must convert data to desired type.

Example Usage:
```
void Example() {
    FindAnyObjectByType<Storage>().DownloadFile("sample.txt", FileType.Algorithm, (bytes) =>
    {
        Debug.Log(Encoding.UTF8.GetString(bytes));
    });
}

```

**Note**: Ensure `FirebaseStorageInstance` has been added to the scene.
