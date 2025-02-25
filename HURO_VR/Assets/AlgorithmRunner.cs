using System.Collections.Generic;
using IronPython.Hosting;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using Microsoft.Scripting.Hosting;
using Unity.VisualScripting;




public class AlgorithmRunner : MonoBehaviour {


    private readonly ScriptEngine engine = Python.CreateEngine();
    private dynamic algorithm;
    bool initAlgorithm = false;
    SceneData sceneData;

    private void Start()
    {

    }


    void SetRobotData(Robot robot, GameObject robot_go)
    {
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
    }

    Robot[] InitRobotData()
    {
        GameObject[] robot_gos = GameObject.FindGameObjectsWithTag("Robot");
        Robot[] robots = new Robot[robot_gos.Length];
        int i = 0;
        foreach (GameObject robot_go in robot_gos)
        {
            robots[i] = new();
            Robot robot = robots[i];
            SetRobotData(robot, robot_go);
            i++;
        }
        return robots;
    }

    void UpdateRobotData()
    {
        GameObject[] robots = GameObject.FindGameObjectsWithTag("Robot");
        foreach (var robot in sceneData.robots)
        {
            foreach (var go in robots)
            {
                if (go.name == robot.name)
                {
                    SetRobotData(robot, go);
                }
            }
        }
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
            obstacles[i].isDynamic = false;

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
            robots = InitRobotData(),
            obstacles = InitObstacles(),
            boundary = InitBoundary()
        };
        sceneData.robot_radius = sceneData.robots[0].radius;

        return sceneData;
    }

    void UpdateSceneData()
    {
        UpdateRobotData();
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

    void InitAlgorithm()
    {
        Debug.Log("Executing Main Function at " + "main.py");
        SetImportPaths(engine);
        algorithm = engine.ExecuteFile(Application.dataPath + @"\main.py");
        sceneData = InitSceneData();
        initAlgorithm = true;
    }

    void RunRVOAlgorithm()
    {
        if (!initAlgorithm)
        {
            InitAlgorithm();
        } else
        {
            UpdateSceneData();
        }
        
        string input = JsonConvert.SerializeObject(sceneData);
        string output = algorithm.main(input);
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

    // Update is called once per frame
    void FixedUpdate () {
        RunRVOAlgorithm();
	}
}
