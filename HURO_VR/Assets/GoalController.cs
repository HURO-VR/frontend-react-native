using UnityEngine;

public class GoalController : MonoBehaviour
{
    GameObject robot;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetRobot(GameObject robot)
    {
        this.robot = robot;
    }

    public bool HasRobot()
    {
        return this.robot != null;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.name + " and " + robot.name);
        if (other.gameObject == this.robot)
        {
            var controller = this.robot.GetComponent<RobotController>();
            controller.GoalReached();
        }
    }
}
