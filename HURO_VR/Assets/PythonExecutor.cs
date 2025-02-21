using UnityEngine;
using System.Diagnostics;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;


public class PythonExecutor : MonoBehaviour
{

    private void Start()
    {
        RunPython();
    }

    void RunPython()
    {
        ScriptEngine engine = Python.CreateEngine();
        engine.Execute("print('Hello from Python')");
        UnityEngine.Debug.Log("Done");
    }

}
