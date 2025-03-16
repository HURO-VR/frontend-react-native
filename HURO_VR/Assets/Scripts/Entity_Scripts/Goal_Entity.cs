using UnityEngine;

public class GoalEntity : MonoBehaviour
{
    GameObject robot;
    // Start is called before the first frame update
    void Awake()
    {
        gameObject.tag = "Goal";
    }

    private void Start()
    {
        if (gameObject.name.ToLower().Contains("flag"))
        {
            gameObject.transform.eulerAngles = new Vector3(-90, 0, 0);
        }
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
