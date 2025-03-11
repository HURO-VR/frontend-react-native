using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public static class SimulationDataCollector
{
    public static SimulationRun simulationRun { get; private set; }
    private static float simulationStartTime;
    private static bool initalized = false;
    public static void InitializeSimulation(SceneData sceneData)
    {
        simulationRun = new SimulationRun
        {
            dateCreated = System.DateTime.UtcNow.ToString("o"),
            status = RunStatus.success,
            starred = false,
            runID = System.Guid.NewGuid().ToString(),
            name = $"Run ..", // Set in Session Controller
            data = new SimulationRunData
            {
                robotData = InitAllRobotData(),
                obstacleData = InitObstacles(sceneData),
                totalCollisions = new List<XYZ>(),
                deadlock = false
            }
        };
        simulationStartTime = Time.time * 1000; // Convert to milliseconds
        initalized = true;
        Debug.Log("HURO: Initalized Data Collector");
    }


    public static void UpdateRobotData()
    {
        GameObject[] robots = GameObject.FindGameObjectsWithTag("Robot");

        foreach (var robotData in simulationRun.data.robotData)
        {
            foreach (var go in robots)
            {
                if (go.name == robotData.name)
                {
                    RobotController robotController = go.GetComponent<RobotController>();
                    robotData.robotPath.Add(Vector3ToXYZ(go.transform.position));

                    if (robotController.IsGoalReached() && !robotData.goalReached)
                    {
                        robotData.goalReached = true;
                        robotData.robotEnd = robotData.goalPosition;
                        robotData.robotPath.Add(robotData.goalPosition);
                    }
                }
            }
        }
    }

    public static void LogServerHit()
    {
        simulationRun.serverHits++;
    }

    public static void LogWarning(string message)
    {
        simulationRun.errorMessage = message;
        simulationRun.status = RunStatus.Warning;
    }

    public static void LogFailed(string message)
    {
        simulationRun.errorMessage = message;
        simulationRun.status = RunStatus.Failed;
    }

    public static void AddCollision(GameObject robot, Collision collision)
    {
        if (!initalized)
        {
            return;
        }
        XYZ collisionPoint = Vector3ToXYZ(robot.transform.position);
        simulationRun.data.totalCollisions.Add(collisionPoint);
        foreach (var ro in simulationRun.data.robotData)
        {
            if (ro.name == robot.name)
            {
                ro.collisions.Add(collisionPoint);
            }
        }
    }

    public static void EndSimulation()
    {
        if (!initalized) {
            Debug.LogWarning("unitialized sim data");
            return; 
        }
        foreach (var ro in simulationRun.data.robotData)
        {
            if (!ro.goalReached)
            {
                ro.robotEnd = ro.robotPath[ro.robotPath.Count - 1];
            }
        }
        simulationRun.data.timeToComplete = (int)((Time.time * 1000) - simulationStartTime);
        simulationRun.data.deadlock = !CheckAllRobotsReachedGoal();
        Debug.Log("HURO: Simulation Ended. Data: " + JsonConvert.SerializeObject(simulationRun, Formatting.Indented));
        initalized = false;
    }

    public static bool CheckAllRobotsReachedGoal()
    {
        if (!initalized) return false;
        foreach (var robot in simulationRun.data.robotData)
        {
            if (!robot.goalReached)
            {
                return false;
            }
        }
        return true;
    }
    static List<RobotData> InitAllRobotData()
    {
        GameObject[] robot_gos = GameObject.FindGameObjectsWithTag("Robot");
        List<RobotData> robots = new List<RobotData>();

        foreach (GameObject robot_go in robot_gos)
        {
            robots.Add(InitRobotData(robot_go));
        }
        return robots;
    }

    static RobotData InitRobotData(GameObject robot_go)
    {
        RobotController robotController = robot_go.GetComponent<RobotController>();

        return new RobotData
        {
            name = robot_go.name,
            robotStart = Vector3ToXYZ(robot_go.transform.position),
            robotPath = new List<XYZ>(),
            goalPosition = Vector3ToXYZ(robotController.GetGoal().transform.position),
            collisions = new List<XYZ>(),
            goalReached = false
        };
    }

    static List<ObstacleData> InitObstacles(SceneData sceneData)
    {

        List<ObstacleData> obstacles = new List<ObstacleData>();
        foreach (Circle go in sceneData.obstacles)
        {
            obstacles.Add(new ObstacleData
            {
                position = go.position,
                radius = go.radius
            });
        }
        return obstacles;
    }


    public static XYZ Vector3ToXYZ(Vector3 v)
    {
        return new XYZ { x = v.x, y = v.y, z = v.z };
    }

    public static Vector3 XYZToVector3(XYZ xyz)
    {
        return new Vector3(xyz.x, xyz.y, xyz.z);
    }
}
