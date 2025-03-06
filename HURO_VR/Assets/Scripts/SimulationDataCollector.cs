using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class SimulationDataCollector : MonoBehaviour
{
    public SimulationRun simulationRun { get; private set; }
    private float simulationStartTime;

    public void InitializeSimulation()
    {
        simulationRun = new SimulationRun
        {
            uid = System.Guid.NewGuid().ToString(),
            dateCreated = System.DateTime.UtcNow.ToString("o"),
            status = RunStatus.success,
            simID = "Sim_" + System.Guid.NewGuid().ToString(),
            starred = false,
            runID = "Run_" + System.Guid.NewGuid().ToString(),
            name = "Simulation Run",
            data = new SimulationRunData
            {
                robotData = InitAllRobotData(),
                obstacleData = InitObstacles(),
                totalCollisions = new List<XYZ>(),
                deadlock = false
            }
        };
        simulationStartTime = Time.time * 1000; // Convert to milliseconds
    }

    List<RobotData> InitAllRobotData()
    {
        GameObject[] robot_gos = GameObject.FindGameObjectsWithTag("Robot");
        List<RobotData> robots = new List<RobotData>();

        foreach (GameObject robot_go in robot_gos)
        {
            robots.Add(InitRobotData(robot_go));
        }
        return robots;
    }

    RobotData InitRobotData(GameObject robot_go)
    {
        RobotController robotController = robot_go.GetComponent<RobotController>();

        return new RobotData
        {
            name = robot_go.name,
            robotStart = Vector3ToXYZ(robot_go.transform.position),
            robotPath = new List<XYZ>(),
            goalPosition = Vector3ToXYZ(robotController.goal.transform.position),
            collisions = new List<XYZ>(),
            goalReached = false
        };
    }

    List<ObstacleData> InitObstacles()
    {
        GameObject[] gos = GameObject.FindGameObjectsWithTag("Obstacle");
        List<ObstacleData> obstacles = new List<ObstacleData>();

        foreach (GameObject go in gos)
        {
            obstacles.Add(new ObstacleData
            {
                position = Vector3ToXYZ(go.transform.position),
                radius = go.GetComponent<Renderer>().bounds.extents.x
            });
        }
        return obstacles;
    }

    public void UpdateRobotData()
    {
        GameObject[] robots = GameObject.FindGameObjectsWithTag("Robot");

        foreach (var robotData in simulationRun.data.robotData)
        {
            foreach (var go in robots)
            {
                if (go.name == robotData.name)
                {
                    robotData.robotPath.Add(Vector3ToXYZ(go.transform.position));

                    if (!robotData.goalReached && Vector3.Distance(go.transform.position, XYZToVector3(robotData.goalPosition)) < 0.5f)
                    {
                        robotData.goalReached = true;
                        robotData.robotEnd = Vector3ToXYZ(go.transform.position);
                    }
                }
            }
        }
    }

    public void DetectCollisions()
    {
        GameObject[] robots = GameObject.FindGameObjectsWithTag("Robot");
        for (int i = 0; i < robots.Length; i++)
        {
            for (int j = i + 1; j < robots.Length; j++)
            {
                if (Vector3.Distance(robots[i].transform.position, robots[j].transform.position) < 1.0f)
                {
                    XYZ collisionPoint = Vector3ToXYZ((robots[i].transform.position + robots[j].transform.position) / 2);
                    simulationRun.data.totalCollisions.Add(collisionPoint);
                    simulationRun.data.robotData[i].collisions.Add(collisionPoint);
                    simulationRun.data.robotData[j].collisions.Add(collisionPoint);
                }
            }
        }
    }

    public bool CheckAllRobotsReachedGoal()
    {
        foreach (var robot in simulationRun.data.robotData)
        {
            if (!robot.goalReached)
            {
                return false;
            }
        }
        return true;
    }

    public void EndSimulation()
    {
        simulationRun.data.timeToComplete = (int)((Time.time * 1000) - simulationStartTime);
        simulationRun.data.deadlock = !CheckAllRobotsReachedGoal();
        Debug.Log("Simulation Ended. Data: " + JsonConvert.SerializeObject(simulationRun, Formatting.Indented));
    }

    private XYZ Vector3ToXYZ(Vector3 v)
    {
        return new XYZ { x = v.x, y = v.y, z = v.z };
    }

    private Vector3 XYZToVector3(XYZ xyz)
    {
        return new Vector3(xyz.x, xyz.y, xyz.z);
    }
}
