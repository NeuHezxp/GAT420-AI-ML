using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
public class RollerAgent : Agent
{
    [SerializeField] Transform target; //gets state of the world
    [SerializeField] float speed = 4;
    [SerializeField] bool useVecObs = true;

    Rigidbody rb;


    public override void CollectObservations(VectorSensor sensor)
    {
        base.CollectObservations(sensor);

        if (useVecObs)
        {
            sensor.AddObservation(target.localPosition); //3 float
            sensor.AddObservation(transform.localPosition); //3 float
            sensor.AddObservation(rb.velocity.x); //1 float
            sensor.AddObservation(rb.velocity.z); //1 float
        }
    }


    public override void OnActionReceived(ActionBuffers actions)
    {
        base.OnActionReceived(actions);
        Vector3 direction = Vector3.zero;
        direction.x = actions.ContinuousActions[0]; //
        direction.z = actions.ContinuousActions[1]; //
        rb.AddForce(direction * speed);

        float distance = Vector3.Distance(transform.localPosition, target.localPosition);
        if (distance < 1)
        {
            SetReward(1);
            EndEpisode();
        }
        if (transform.localPosition.y < 0)
        {
            SetReward(-1);
            EndEpisode();
        }
    }

    public override void OnEpisodeBegin()
    {
        base.OnEpisodeBegin();
        //reset the agent
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        transform.localPosition = Vector3.up;

        target.localPosition = new Vector3(Random.Range(-4, 4), 1, Random.Range(-4, 4));
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    public override void Heuristic(in ActionBuffers actionsOut) //controls the agent while testing
    {
        base.Heuristic(actionsOut);
        var continousAction = actionsOut.ContinuousActions;
        continousAction[0] = Input.GetAxis("Horizontal");
        continousAction[1] = Input.GetAxis("Vertical");

    }
}
