using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;

public class MazeAgent : Agent
{
    [SerializeField] Transform target; // Reference to the target object the agent should find.
    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform targetPoint; // Reference to the target's transform.
    [SerializeField] float speed = 1; // Speed of the agent movement.
    [SerializeField] float turnRate = 360; // Rate at which the agent can turn.


    Rigidbody rb;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Called when an action is received from either the neural network or the heuristic.
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        base.OnActionReceived(actionBuffers); // Call the base implementation.

        float force = actionBuffers.ContinuousActions[1] * speed; // Get the forward/backward force from the action buffer.
        rb.AddRelativeForce(Vector3.forward * force, ForceMode.VelocityChange); // Apply the force to move the agent.

        // Turn the agent based on the action received.
        float turn = actionBuffers.ContinuousActions[0] * turnRate; // Get the turn rate from the action buffer.
        transform.rotation = transform.rotation * Quaternion.AngleAxis(turn * Time.deltaTime, Vector3.up); // Apply the rotation.

        // Add a negative reward every step to encourage faster completion.
        AddReward(-1.0f / MaxStep);
    }
    // Called at the beginning of each episode, reset the agent and environment.
    public override void OnEpisodeBegin()
    {
        base.OnEpisodeBegin(); // Call the base implementation.

        // Set the agent's position and rotation to that of the StartPoint object.
        if (startPoint != null)
        {
            transform.position = startPoint.position;
            transform.rotation = startPoint.rotation;
        }
        else
        {
            Debug.LogWarning("StartPoint not assigned in MazeAgent script.");
        }

        rb.velocity = Vector3.zero; // Reset velocity.
        rb.angularVelocity = Vector3.zero; // Reset angular velocity.

        // Set the target's position and rotation to that of the TargetPoint object.
        if (targetPoint != null)
        {
            target.localPosition = targetPoint.position;
            // Optionally, adjust the target's rotation as well:
            target.rotation = targetPoint.rotation;
        }
        else
        {
            Debug.LogWarning("TargetPoint not assigned in MazeAgent script.");
        }
    }

    // Provides manual control for testing and debugging.
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        base.Heuristic(actionsOut);
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal"); // Left/right input.
        continuousActionsOut[1] = Input.GetAxis("Vertical"); // Forward/backward input.
    }

    // Collision detection for rewards and episode termination.
    private void OnCollisionEnter(Collision collision)
    {

        int obstacleLayer = LayerMask.NameToLayer("Obstacle");
        int targetLayer = LayerMask.NameToLayer("Target");

        // Check if the collided object is an obstacle
        if (collision.gameObject.layer == obstacleLayer)
        {
            AddReward(-1);
            EndEpisode();
        }

        // Check if the collided object is the target
        if (collision.gameObject.layer == targetLayer)
        {
            AddReward(5);
            EndEpisode();
        }
    }
}
