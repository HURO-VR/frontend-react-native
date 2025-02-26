using System.Collections;
using System.Collections.Generic;
using Meta.XR.MRUtilityKit;
using UnityEngine;

public class SceneDataManager : MonoBehaviour
{

    public SceneData data;
    MRUKRoom mruk;
    public MRUKAnchor.SceneLabels mrukObstacleLabel;
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

    void LabelMRObstacles()
    {
        foreach (MRUKAnchor anchor in mruk.Anchors)
        {
            if ((mrukObstacleLabel & anchor.Label) != 0)
            {
                Renderer mesh = anchor.gameObject.GetComponentInChildren<Renderer>();
                if (mesh != null) mesh.gameObject.tag = "Obstacle";
            }
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
        Renderer renderer;
        if (floor == null) floor = mruk.FloorAnchor.gameObject.GetComponentInChildren<Renderer>().gameObject;
        renderer = floor.GetComponent<Renderer>();
        if (renderer == null)
        {
            Debug.LogWarning("Scene floor not detected. No boundary set.");
            return null;
        }
        boundary.position.x = floor.transform.localPosition.x;
        boundary.position.y = floor.transform.localPosition.y;
        boundary.position.z = floor.transform.localPosition.z;
        boundary.width = renderer.bounds.size.x;
        boundary.length = renderer.bounds.size.z;

        return boundary;

    }

    Obstacle[] InitObstacles()
    {
        if (mruk) LabelMRObstacles();
        GameObject[] gos = GameObject.FindGameObjectsWithTag("Obstacle");
        Obstacle[] obstacles = new Obstacle[gos.Length];
        int i = 0;
        foreach (GameObject go in gos)
        {
            obstacles[i] = new Obstacle();
            obstacles[i].SetData(go);
            i++;
        }
        return obstacles;
    }

    public void InitSceneData()
    {
        data.robots = InitRobotData();
        Debug.Log("Initalized Robots: " + data.robots.Length);

        data.obstacles = InitObstacles();
        Debug.Log("Initalized Obstacles: " + data.obstacles.Length);

        data.boundary = InitBoundary();
        Debug.Log("Initalized Boundary of size " + data.boundary.length + " by " + data.boundary.width);

        data.robot_radius = data.robots[0].radius;
        Debug.Log("Initalized Robot Radius: " + data.robot_radius);

    }

    public void UpdateSceneData()
    {
        UpdateRobotData();
    }
}
