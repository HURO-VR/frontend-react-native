using System.Collections.Generic;
using IronPython.Hosting;
using UnityEngine;
using Newtonsoft.Json;




public class AlgorithmRunner : MonoBehaviour {

    string ALGORITHM_PATH = @"\Resources\rvo_algorithm\";
    string ALGORITHM_NAME = "main.py";
    private void Start()
    {
        RunPythonFile(ALGORITHM_PATH, ALGORITHM_NAME);
    }

    Robot InitRobotData()
    {
        Robot robot = new Robot();
        GameObject robot_go = GameObject.FindGameObjectWithTag("Robot");
        Transform robot_transform = robot_go.transform;
        Vector3 robot_velocity = robot_go.GetComponent<Rigidbody>().velocity;
        RobotController robotController = robot_go.GetComponent<RobotController>();
        Transform goal_transform = robotController.goal.transform;

        robot.position.x = robot_transform.position.x;
        robot.position.y = robot_transform.position.y;
        robot.position.z = robot_transform.position.z;

        robot.curr_velocity.x = robot_velocity.x;
        robot.curr_velocity.y = robot_velocity.y;
        robot.curr_velocity.z = robot_velocity.z;

        robot.goal.x = goal_transform.position.x;
        robot.goal.y = goal_transform.position.y;
        robot.goal.z = goal_transform.position.z;

        robot.max_velocity = robotController.maxVelocity;
        robot.radius = 1;

        return robot;
    }
    Boundary InitBoundary()
    {
        Boundary boundary = new Boundary(); 

        return boundary;

    }

    Obstacle InitObstacle()
    {
        Obstacle obstacle = new Obstacle();

        return obstacle;
    }

    SceneData InitSceneData()
    {
        SceneData sceneData = new()
        {
            robots = new Robot[1],
            obstacles = new Obstacle[1],
            boundary = InitBoundary()
        };

        sceneData.robots[0] = InitRobotData();        
        sceneData.obstacles[0] = InitObstacle();
        sceneData.robot_radius = sceneData.robots[0].radius;

        return sceneData;
    }


    // Use this for initialization
    void RunPythonFile(string path, string filename)
    {
        if (!path.EndsWith(@"\")) path += @"\";
        if (!path.StartsWith(@"\")) path.Insert(0, @"\");

        var engine = Python.CreateEngine();

        ICollection<string> searchPaths = engine.GetSearchPaths();
        
        //Path to the folder of greeter.py
        searchPaths.Add(Application.dataPath + path);
        //Path to the Python standard library
        searchPaths.Add(Application.dataPath + @"\Plugins\IronPy\Lib\");
        engine.SetSearchPaths(searchPaths);

        dynamic py = engine.ExecuteFile(Application.dataPath + path + filename);
        string json = JsonConvert.SerializeObject(InitSceneData());

        Debug.Log("Executing Main Function at " + path);
        Debug.Log(py.main(json));
    }

    // Update is called once per frame
    void Update () {
		
	}
}
