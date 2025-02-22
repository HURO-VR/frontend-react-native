using System.Collections.Generic;
using IronPython.Hosting;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using Microsoft.Scripting.Hosting;




public class AlgorithmRunner : MonoBehaviour {

    private void Start()
    {
    }

    Robot InitRobotData()
    {
        Robot robot = new Robot();
        GameObject robot_go = GameObject.FindGameObjectWithTag("Robot");
        Transform robot_transform = robot_go.transform;
        Vector3 robot_velocity = robot_go.GetComponent<Rigidbody>().velocity;
        RobotController robotController = robot_go.GetComponent<RobotController>();
        SphereCollider sphereCollider = robot_go.GetComponent<SphereCollider>();

        Transform goal_transform = robotController.goal.transform;
        Renderer renderer = robot_transform.GetComponent<Renderer>();
        robot.name = robot_go.name;

        robot.position.x = robot_transform.localPosition.x;
        robot.position.y = robot_transform.localPosition.y;
        robot.position.z = robot_transform.localPosition.z;

        robot.curr_velocity.x = robot_velocity.x;
        robot.curr_velocity.y = robot_velocity.y;
        robot.curr_velocity.z = robot_velocity.z;

        robot.goal.x = goal_transform.localPosition.x;
        robot.goal.y = goal_transform.localPosition.y;
        robot.goal.z = goal_transform.localPosition.z;

        robot.max_velocity = robotController.maxVelocity;
        robot.radius = renderer.bounds.size.x / 2;

        return robot;
    }
    Boundary InitBoundary()
    {
        Boundary boundary = new Boundary();
        GameObject floor = GameObject.FindGameObjectWithTag("Floor");
        Renderer renderer = floor.GetComponent<Renderer>();
        boundary.position.x = floor.transform.localPosition.x;
        boundary.position.y = floor.transform.localPosition.y;
        boundary.position.z= floor.transform.localPosition.z;
        boundary.width = renderer.bounds.size.x;
        boundary.length = renderer.bounds.size.z;

        return boundary;

    }

    Obstacle[] InitObstacles()
    {
        GameObject[] gos = GameObject.FindGameObjectsWithTag("Obstacle");
        Obstacle[] obstacles = new Obstacle[gos.Length];
        int i = 0;
        foreach (GameObject go in gos) {
            obstacles[i] = new Obstacle();
            obstacles[i].position.x = go.transform.localPosition.x;
            obstacles[i].position.y = go.transform.localPosition.y;
            obstacles[i].position.z = go.transform.localPosition.z;

            Renderer renderer = go.GetComponent<Renderer>();
            obstacles[i].radius = Mathf.Max(renderer.bounds.size.z, renderer.bounds.size.x);
            i++;
        }

        return obstacles;
    }

    SceneData InitSceneData()
    {
        SceneData sceneData = new()
        {
            robots = new Robot[1],
            obstacles = InitObstacles(),
            boundary = InitBoundary()
        };

        sceneData.robots[0] = InitRobotData();        
        sceneData.robot_radius = sceneData.robots[0].radius;

        return sceneData;
    }

    void SetImportPaths(ScriptEngine engine)
    {
        ICollection<string> searchPaths = engine.GetSearchPaths();

        //Path to the folder of filename
        searchPaths.Add(Application.dataPath);

        //Path to the Python standard library
        searchPaths.Add(Application.dataPath + @"\Plugins\IronPy\Lib\");

        engine.SetSearchPaths(searchPaths);
    }


    // Use this for initialization
    void RunRVOAlgorithm()
    {
        var engine = Python.CreateEngine();

        SetImportPaths(engine);

        dynamic py = engine.ExecuteFile(Application.dataPath + @"\main.py");
        //Debug.Log("Executing Main Function at " + "main.py");
        var sceneData = InitSceneData();
        string json = JsonConvert.SerializeObject(sceneData);
        //Debug.Log(json);
        string output = py.main(json);
        //Debug.Log(output);
        var newVelocities = JsonConvert.DeserializeObject<float[][]>(output);
        SetNewVelocities(newVelocities, sceneData.robots);
    }

    void SetNewVelocities(float[][] newVelocities, Robot[] robots)
    {
        GameObject[] robots_go = GameObject.FindGameObjectsWithTag("Robot");
        foreach (var go in robots_go)
        {
            int i = 0;
            foreach (var robot in robots)
            {
                if (go.name == robot.name)
                {
                    go.GetComponent<Rigidbody>().velocity = new Vector3(newVelocities[i][0], 0, newVelocities[i][1]);
                }
                i++;
            }
        }
    }

    int cycles = 0;
    // Update is called once per frame
    void FixedUpdate () {
        if (cycles % 10 != 0)
        {
            cycles++;
            return;
        }
        RunRVOAlgorithm();
        cycles = 1;
	}
}
