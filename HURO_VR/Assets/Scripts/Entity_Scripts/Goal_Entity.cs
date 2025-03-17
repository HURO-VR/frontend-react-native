using UnityEngine;

public class GoalEntity : MonoBehaviour
{
    GameObject robot;
    [SerializeField] Vector3 initialRotation;
    // Start is called before the first frame update
    void Awake()
    {
        gameObject.tag = "Goal";
    }

    private void Start()
    {
        gameObject.transform.eulerAngles = initialRotation;
        
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
            var controller = this.robot.GetComponent<RobotEntity>();
            controller.GoalReached();
        }
    }
}
