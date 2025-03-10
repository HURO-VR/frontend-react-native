using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class RobotController : MonoBehaviour
{

    GameObject goal;
    public float maxVelocity;
    bool goalReached = false;
    Rigidbody body;
    public bool stuck { get; private set; }
    AlgorithmRunner algorithmRunner;


    private void Awake()
    {
        stuck = false;
        algorithmRunner = FindAnyObjectByType<AlgorithmRunner>();
        body = GetComponent<Rigidbody>();
    }
    // Start is called before the first frame update
    void Start()
    {
        if (goal == null)
        {
            InitGoal(); 
        }
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

    public bool IsGoalReached()
    {
        return goalReached;
    }

    float timer = 0;
    [SerializeField] float deadlockLimit = 5f;

    public void SetDeadlockTimer(float deadlockLimit)
    {
        this.deadlockLimit = deadlockLimit;
    }

    bool IsStuck()
    {
        if (!algorithmRunner.IsRunning()) return false;
        return body.velocity.magnitude < 0.1f;

    }

    // Update is called once per frame
    void Update()
    {
        if (!goal) InitGoal();
        if (IsStuck())
        {
            timer += Time.deltaTime;
            if (timer > deadlockLimit) stuck = true;
        } else
        {
            stuck = false;
            timer = 0;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.name.ToLower().Contains("floor")) SimulationDataCollector.AddCollision(gameObject, collision);
    }
}
