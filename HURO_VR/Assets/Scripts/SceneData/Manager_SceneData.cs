using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Meta.XR.MRUtilityKit;
using Newtonsoft.Json;
using UnityEngine;

public class SceneDataManager : MonoBehaviour
{
    #region Serialized Variables

    // Add any [SerializeField] variables here if needed

    #endregion

    #region Public Variables

    public SceneData data;
    public MRUKAnchor.SceneLabels mrukObstacleLabel;

    #endregion

    #region Private Variables

    private bool initalized = false;
    private MRUKRoom mruk;

    #endregion

    #region Unity Methods

    void Start()
    {
        data = new SceneData();
        mruk = FindObjectOfType<MRUKRoom>();
    }

    #endregion

    #region Public Methods

    public void InitSceneData()
    {
        data ??= new SceneData();
        if (!mruk) mruk = FindObjectOfType<MRUKRoom>();
        if (mruk) LabelMRObjects();

        if (initalized) return;
        data.robots = InitRobotData();
        Debug.Log("Initalized Robots: " + data.robots.Length);

        Robot[] goals = data.robots.Where((robot) => robot.goal.x != 0).ToArray();
        Debug.Log("Initalized Goals: " + goals.Length + " position: " + goals[0].goal);

        data.obstacles = InitObstacles();
        Debug.Log("Initalized Obstacles: " + data.obstacles.Length);

        data.boundary = new Boundary(GameObject.FindGameObjectWithTag("Floor"));
        Debug.Log("Initalized Boundary of size " + data.boundary.length + " by " + data.boundary.width + " and position: " + data.boundary.position);

        data.robot_radius = data.robots[0].radius;
        Debug.Log("Initalized Robot Radius: " + data.robot_radius);

        data.LoadOutput();

        initalized = true;
    }

    public string GetAlgorithmInput()
    {
        return JsonConvert.SerializeObject(data.LoadOutput());
    }

    public void UpdateSceneData()
    {
        UpdateRobotData();
        UpdateObstacleData();
    }

    public void DrawGizmos()
    {
        if (data == null) return;
        if (data.obstacles != null) 
            foreach (var obstacle in data.obstacles)
            {
                obstacle.DrawGizmo();
            }

        if (data.boundary != null) data.boundary.DrawGizmo();
        if (data.robots != null) foreach (var robot in data.robots)
            {
                robot.DrawGizmo();
            }
    }

    #endregion

    #region Private Methods

    private void LabelMRObjects()
    {
        bool floor = false;
        foreach (MRUKAnchor anchor in mruk.Anchors)
        {
            Renderer mesh = anchor.gameObject.GetComponentInChildren<Renderer>();
            if ((mrukObstacleLabel & anchor.Label) != 0)
            {
                if (mesh != null) mesh.gameObject.tag = "Obstacle";
            }

            if ((anchor.Label & MRUKAnchor.SceneLabels.FLOOR) != 0)
            {
                mesh.gameObject.tag = "Floor";
                floor = true;
            }
        }

        if (!floor)
        {
            mruk.FloorAnchor.gameObject.GetComponentInChildren<Renderer>().gameObject.tag = "Floor";
        }
    }

    private Robot[] InitRobotData()
    {
        GameObject[] robot_gos = GameObject.FindGameObjectsWithTag("Robot");
        Robot[] robots = new Robot[robot_gos.Length];
        int i = 0;
        foreach (GameObject robot_go in robot_gos)
        {
            robots[i] = new Robot(robot_go);
            i++;
        }
        return robots;
    }

    private void UpdateRobotData()
    {
        GameObject[] robots = GameObject.FindGameObjectsWithTag("Robot");
        foreach (var robot in data.robots)
        {
            foreach (var go in robots)
            {
                if (go.name == robot.name)
                {
                    robot.SetData(go);
                }
            }
        }
    }

    private Obstacle[] InitObstacles()
    {
        GameObject[] gos = GameObject.FindGameObjectsWithTag("Obstacle");
        Obstacle[] obstacles = new Obstacle[gos.Length];
        int i = 0;
        foreach (GameObject go in gos)
        {
            obstacles[i] = new Obstacle(go);
            i++;
        }
        return obstacles;
    }

    private void UpdateObstacleData()
    {
        foreach (Obstacle obstacle in data.obstacles)
        {
            if (obstacle.isDynamic)
            {
                obstacle.position = obstacle.go.transform.position;
                if (obstacle.circleAbstraction != null)
                {
                    obstacle.CreateCircleAbstraction();
                }
            }
        }
    }

    #endregion

    
}