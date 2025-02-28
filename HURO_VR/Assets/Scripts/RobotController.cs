using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotController : MonoBehaviour
{

    GameObject goal;
    public float maxVelocity;
    bool goalReached = false;
    Rigidbody body;

    // Start is called before the first frame update
    void Start()
    {
        if (goal == null)
        {
            InitGoal(); 
        }
        body = GetComponent<Rigidbody>();
        if (maxVelocity == 0) maxVelocity = 2;
    }

    public void SetVelocity(float x, float z)
    {
        if (!goalReached)
        {
            body.velocity = new Vector3(x, 0, z);
        } else
        {
            Debug.Log(gameObject.name + " not responding to further input");
        }
    }

    public void GoalReached()
    {
        Debug.LogWarning(gameObject.name + " reached goal.");
        goalReached = true;
        body.velocity = Vector3.zero;
        body.isKinematic = true;
    }

    public GameObject GetGoal()
    {
        if (goal == null)
        {
            InitGoal();
        }
        return goal;
    }

    void InitGoal()
    {
        GoalController[] goals = FindObjectsByType<GoalController>(FindObjectsSortMode.InstanceID);
        foreach (GoalController goal in goals)
        {
            if (!goal.HasRobot())
            {
                goal.SetRobot(gameObject);
                this.goal = goal.gameObject;
                return;
            }
        }
        Debug.LogWarning("Robot " + gameObject.name + " does not have goal.");
    }

    // Update is called once per frame
    void Update()
    {
        if (!goal) InitGoal();
    }
}
