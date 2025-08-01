using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallShooterCPU : MonoBehaviour
{
    [System.Serializable]
    public class PositionTargetPair
    {
        public Transform directTarget;
        public Transform backboardTarget;
    }

    [Header("Target")]
    public Transform directTarget;
    public Transform backboardTarget;
    public List<PositionTargetPair> positionTargetPairs;
    public PositionManager positionManager;


    [Range(0f, 1f)]
    public float useBackboardChance = 0.5f;

    [Header("Traiettoria")]
    public float arcHeight = 2.0f;
    public float forceMultiplier = 1.0f;

    [Header("Precisione")]
    [Range(0f, 1f)]
    public float precision = 1.0f;
    public float maxDeviationAngle = 6f;
    public float maxVerticalDeviation = 1.0f;

    [Header("Palla")]
    public Rigidbody ball;

    [Header("Effetto/Spin")]
    public float backspinAmount = 10f;
    public float curveSpinAmount = 3f;

    private Vector3 chosenTarget;
    private float chosenArcHeight;

    
    public void ShootCPU(GameObject ballInstance, float force, float prec, Vector2 swipeStart, Vector2 swipeEnd, int target, float curvatureFactor = 0f)
    {
        ball = ballInstance.GetComponent<Rigidbody>();
        forceMultiplier = force;
        precision = prec;
        ball.useGravity = true;

        switch (target)
        {
            case 0:
                chosenTarget = GetIdealTarget();
                chosenArcHeight = arcHeight;
                break;
            case 1:
                chosenTarget = positionTargetPairs[positionManager.GetIndexCPU()].directTarget.position;
                chosenArcHeight = positionTargetPairs[positionManager.GetIndexCPU()].directTarget.GetComponent<TargetManager>().arcHeightToShoot;
                break;
            case 2:
                chosenTarget = positionTargetPairs[positionManager.GetIndexCPU()].backboardTarget.position;
                chosenArcHeight = positionTargetPairs[positionManager.GetIndexCPU()].backboardTarget.GetComponent<TargetManager>().arcHeightToShoot;
                break;
            default:
                chosenTarget = GetIdealTarget();
                chosenArcHeight = arcHeight;
                break;
        }

        Vector3 adjustedTarget = chosenTarget;

        if (CalculateArcVelocity(transform.position, adjustedTarget, chosenArcHeight, out Vector3 velocity))
        {
            velocity *= forceMultiplier;
            ball.velocity = velocity;
        }
        else
        {
            Debug.LogWarning("Traiettoria non calcolabile");
            Vector3 fallbackDir = (adjustedTarget - transform.position).normalized;
            Vector3 fallbackVelocity = fallbackDir * 5f + Vector3.up * 4f; // valori sperimentali

            ball.velocity = fallbackVelocity;
            Debug.LogWarning("Tiro approssimativo eseguito");

        }
    }

    private Vector3 GetIdealTarget()
    {
        return Random.value < useBackboardChance ? backboardTarget.position : directTarget.position;
    }

    private Vector3 ApplyInaccuracy(Vector3 originalTarget, Vector2 swipeStart, Vector2 swipeEnd)
    {
        Vector3 direction = originalTarget - transform.position;
        float distance = direction.magnitude;

        // 1. Deviazione angolare basata sullo swipe (da -1 a 1)
        float swipeXRatio = Mathf.Clamp((swipeEnd.x - swipeStart.x) / (swipeEnd.y - swipeStart.y + 0.01f), -1f, 1f);

        // 2. Calcolo deviazione laterale proporzionale al maxDeviationAngle
        float deviationAngle = swipeXRatio * (1f - precision) * maxDeviationAngle;

        Quaternion rot = Quaternion.Euler(0f, deviationAngle, 0f);
        Vector3 deviatedDir = rot * direction;

        Vector3 finalTarget = transform.position + deviatedDir.normalized * distance;

        // 3. Deviazione verticale casuale
        float verticalDeviation = (1f - precision) * maxVerticalDeviation;
        float verticalOffset = Random.Range(-verticalDeviation, verticalDeviation);
        finalTarget.y += verticalOffset;

        return finalTarget;
    }

    private bool CalculateArcVelocity(Vector3 origin, Vector3 target, float arcHeight, out Vector3 velocity)
    {

        velocity = Vector3.zero;

        if (precision < 1f)
        {
            float maxHorizontalOffset = 0.2f * (1f - precision); // max errore laterale 0.2 m
            float maxVerticalOffset = 0.1f * (1f - precision);   // max errore verticale 0.1 m

            Vector3 randomOffset = new Vector3(
                Random.Range(-maxHorizontalOffset, maxHorizontalOffset),
                Random.Range(-maxVerticalOffset, maxVerticalOffset),
                Random.Range(-maxHorizontalOffset, maxHorizontalOffset)
            );

            target += randomOffset;
        }


        Vector3 toTarget = target - origin;
        Vector3 horizontal = new Vector3(toTarget.x, 0f, toTarget.z);
        float distance = horizontal.magnitude;

        velocity = Vector3.zero;
        if (distance < 0.01f) return false;

        float heightDifference = toTarget.y;
        float g = Mathf.Abs(Physics.gravity.y);

        float timeUp = Mathf.Sqrt(2 * arcHeight / g);
        float timeDown = Mathf.Sqrt(2 * Mathf.Max(arcHeight - heightDifference, 0.01f) / g);
        float totalTime = timeUp + timeDown;

        if (totalTime <= 0.01f) return false;

        Vector3 velocityY = Vector3.up * Mathf.Sqrt(2 * g * arcHeight);
        Vector3 velocityXZ = horizontal / totalTime;

        velocity = velocityXZ + velocityY;
        return !float.IsNaN(velocity.x) && !float.IsNaN(velocity.y) && !float.IsNaN(velocity.z);
    }

}
