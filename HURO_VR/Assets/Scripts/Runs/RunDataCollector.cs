using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public static class RunDataCollector
{
    public static RunMetadata runMetadata { get; private set; }
    private static float simulationStartTime;
    private static bool initalized = false;
    public static void InitializeSimulation(SceneDataManager.SceneDataOutput sceneData)
    {
        runMetadata = new RunMetadata
        {
            dateCreated = System.DateTime.UtcNow.ToString("o"),
            status = RunStatus.success,
            starred = false,
            runID = System.Guid.NewGuid().ToString(),
            name = $"Run ..", // Set in Session Controller
            data = new RunData
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

        foreach (var robotData in runMetadata.data.robotData)
        {
            foreach (var go in robots)
            {
                if (go.name == robotData.name)
                {
                    RobotEntity robotController = go.GetComponent<RobotEntity>();
                    robotData.robotPath.Add(go.transform.position);

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
        runMetadata.serverHits++;
    }

    public static void LogWarning(string message)
    {
        runMetadata.errorMessage = message;
        runMetadata.status = RunStatus.Warning;
    }

    public static void LogFailed(string message)
    {
        runMetadata.errorMessage = message;
        runMetadata.status = RunStatus.Failed;
    }

    public static void AddCollision(GameObject robot, Collision collision)
    {
        if (!initalized)
        {
            return;
        }
        XYZ collisionPoint = robot.transform.position;
        runMetadata.data.totalCollisions.Add(collisionPoint);
        foreach (var ro in runMetadata.data.robotData)
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
        foreach (var ro in runMetadata.data.robotData)
        {
            if (!ro.goalReached)
            {
                ro.robotEnd = ro.robotPath[ro.robotPath.Count - 1];
            }
        }
        runMetadata.data.timeToComplete = (int)((Time.time * 1000) - simulationStartTime);
        runMetadata.data.deadlock = !CheckAllRobotsReachedGoal();
        Debug.Log("HURO: Simulation Ended. Data: " + JsonConvert.SerializeObject(runMetadata, Formatting.Indented));
        initalized = false;
    }

    public static bool CheckAllRobotsReachedGoal()
    {
        if (!initalized) return false;
        foreach (var robot in runMetadata.data.robotData)
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
        RobotEntity robotController = robot_go.GetComponent<RobotEntity>();

        return new RobotData
        {
            name = robot_go.name,
            robotStart = robot_go.transform.position,
            robotPath = new List<XYZ>(),
            goalPosition = robotController.GetGoal().transform.position,
            collisions = new List<XYZ>(),
            goalReached = false
        };
    }

    static List<ObstacleData> InitObstacles(SceneDataManager.SceneDataOutput sceneData)
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

}
