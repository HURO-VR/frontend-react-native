using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine.UIElements.Experimental;

// Input format (Vector3 selfPosition, Vector3 targetPosition). 6 Dimensions
public class SelfAndTargetModelHarness : AgentWrapper
{

    [SerializeField] Transform targetTransform;
    [SerializeField] float moveSpeed;
    [SerializeField] Material successMaterial;
    [SerializeField] Material failMaterial;
    [SerializeField] Renderer floor;


    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];
        transform.localPosition += new Vector3(moveX, 0, moveZ) * Time.deltaTime * moveSpeed;
    }

    public override void OnEpisodeBegin()
    {
        this.LogRun();
        float initialX = Random.Range(-4.9f, 4.9f);
        float initialZ = Random.Range(-4.9f, 4.9f);
        float initialTargetX = Random.Range(-4.9f, 4.9f);
        float initialTargetZ = Random.Range(-4.9f, 4.9f);
        transform.localPosition = new Vector3(initialX, .5f, initialZ);
        targetTransform.localPosition = new Vector3(initialTargetX, .5f, initialTargetZ);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(targetTransform.localPosition);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name.Contains("target"))
        {
            SetReward(1);
            this.LogSuccess();
            floor.material = successMaterial ;
            EndEpisode();
        }
    }


    private void Update()
    {
        if (Mathf.Max(Mathf.Abs(transform.localPosition.x), Mathf.Abs(transform.localPosition.z)) >= 5)
        {
            SetReward(-1);
            this.LogFail();
            floor.material = failMaterial;
            EndEpisode();
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> actionSegment = actionsOut.ContinuousActions;
        actionSegment[0] = Input.GetAxisRaw("Horizontal");
        actionSegment[1] = Input.GetAxisRaw("Vertical");
    }

}
