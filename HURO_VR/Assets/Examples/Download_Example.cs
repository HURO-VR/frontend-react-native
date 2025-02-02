using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class DownloadExample : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        FindAnyObjectByType<Storage>().DownloadFile("sample.txt", FileType.Algorithm, (bytes) =>
        {
            Debug.Log(Encoding.UTF8.GetString(bytes));
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
