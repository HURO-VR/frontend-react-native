using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Meta.XR.MRUtilityKit;
using UnityEngine;

public class SceneDataManager : MonoBehaviour
{

    public SceneData data;
    MRUKRoom mruk;
    public MRUKAnchor.SceneLabels mrukObstacleLabel;
    bool initalized = false;
    // Start is called before the first frame update
    void Start()
    {
        data = new SceneData();
        mruk = FindObjectOfType<MRUKRoom>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void LabelMRObjects()
    {
        bool floor = false;
        foreach (MRUKAnchor anchor in mruk.Anchors)
        {
            Renderer mesh = anchor.gameObject.GetComponentInChildren<Renderer>();
            if ((mrukObstacleLabel & anchor.Label) != 0)
            {
                if (mesh != null) mesh.gameObject.tag = "Obstacle";
            }

            if ((anchor.Label & MRUKAnchor.SceneLabels.FLOOR) != 0) {
                mesh.gameObject.tag = "Floor";
                floor = true;
            }
        }

        if (!floor)
        {
            mruk.FloorAnchor.gameObject.GetComponentInChildren<Renderer>().gameObject.tag = "Floor";
        }
    }

    Robot[] InitRobotData()
    {
        GameObject[] robot_gos = GameObject.FindGameObjectsWithTag("Robot");
        Robot[] robots = new Robot[robot_gos.Length];
        int i = 0;
        foreach (GameObject robot_go in robot_gos)
        {
            robots[i] = new Robot();
            robots[i].SetData(robot_go);
            i++;
        }
        return robots;
    }

    void UpdateRobotData()
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

    Boundary InitBoundary()
    {
        Boundary boundary = new Boundary();
        GameObject floor = GameObject.FindGameObjectWithTag("Floor");
        Renderer renderer = floor.GetComponent<Renderer>();
        if (renderer == null)
        {
            Debug.LogWarning("Scene floor not detected. No boundary set.");
            return null;
        }

        boundary.position.x = floor.transform.position.x;
        boundary.position.y = floor.transform.position.y;
        boundary.position.z = floor.transform.position.z;

        boundary.width = renderer.bounds.size.x;
        boundary.length = renderer.bounds.size.z;

        return boundary;

    }

    Obstacle[] InitObstacles()
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

        data.fullObstacles = InitObstacles();
        Debug.Log("Initalized Obstacles: " + data.fullObstacles.Length);

        data.obstacles = Obstacle.UnpackAbstractions(data.fullObstacles);

        data.boundary = InitBoundary();
        Debug.Log("Initalized Boundary of size " + data.boundary.length + " by " + data.boundary.width + " and position: " + data.boundary.position);

        data.robot_radius = data.robots[0].radius;
        Debug.Log("Initalized Robot Radius: " + data.robot_radius);

        initalized = true;

    }

    public void UpdateSceneData()
    {
        UpdateRobotData();
    }

    public void DrawGizmos()
    {
        if (data == null) return;
        if (data.obstacles != null) foreach (var obstacle in data.fullObstacles)
        {
            obstacle.DrawGizmo();
        }

        if (data.boundary != null) data.boundary.DrawGizmo();
        if (data.robots != null) foreach (var robot in  data.robots)
        {
            robot.DrawGizmo();
        }
    }

}
