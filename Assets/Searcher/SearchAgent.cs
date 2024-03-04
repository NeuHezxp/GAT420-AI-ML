using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;

public class SearchAgent : Agent
{
    [SerializeField] Transform target;
    [SerializeField] float speed = 1;
    [SerializeField] float turnRate = 360;

    Rigidbody rb;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        base.OnActionReceived(actionBuffers);

        float force = actionBuffers.ContinuousActions[1] * speed;
        rb.AddRelativeForce(Vector3.forward * force, ForceMode.VelocityChange);

        //turn agent
        float turn = actionBuffers.ContinuousActions[0] * turnRate;
        transform.rotation = transform.rotation * Quaternion.AngleAxis(turn * Time.deltaTime, Vector3.up);

        //reduce the reward over time to make agent find the target faster
        AddReward(-1.0f / MaxStep);
    }
    public override void OnEpisodeBegin()
    {
        base.OnEpisodeBegin();
        // set the agent at random position and rotation
        transform.localPosition = new Vector3(Random.Range(-15, 15), 1.5f, Random.Range(-15, 15));
        transform.rotation = Quaternion.AngleAxis(Random.value * 360, Vector3.up);
        // reset agent rigidbody
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        // set the target at a random position on the floor
        target.localPosition = new Vector3(Random.Range(-15, 15), 1.5f, Random.Range(-15, 15));

    }
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        base.Heuristic(actionsOut);
        // allow input control of the agent
        var continousActionsOut = actionsOut.ContinuousActions;
        continousActionsOut[0] = Input.GetAxis("Horizontal");
        continousActionsOut[1] = Input.GetAxis("Vertical");
    }
    private void OnCollisionEnter(Collision collision)
    {
        // stop the episode if an obstacle was hit, give negative reward
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            AddReward(-1);
            EndEpisode();
        }

        if (collision.gameObject.CompareTag("Target"))
        {
            AddReward(5);
            EndEpisode();
        }
    }
}
