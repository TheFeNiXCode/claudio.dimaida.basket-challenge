using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallShooter : MonoBehaviour
{
    [Header("Target")]
    public Transform directTarget;
    public Transform backboardTarget;

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

    public void Shoot(GameObject ballInstance, float force, float prec, Vector2 swipeStart, Vector2 swipeEnd, float curvatureFactor = 0f)
    {
        ball = ballInstance.GetComponent<Rigidbody>();
        forceMultiplier = force;
        precision = prec;
        ball.useGravity = true;


        Vector3 chosenTarget = GetIdealTarget();
        Vector3 adjustedTarget = ApplyInaccuracy(chosenTarget, swipeStart, swipeEnd);

        if (CalculateArcVelocity(transform.position, adjustedTarget, arcHeight, out Vector3 velocity))
        {
            velocity *= forceMultiplier;
            ball.velocity = velocity;
            //ApplySpinSimple(velocity); // Aggiunta della rotazione!
            //ApplySpin(transform.position, adjustedTarget, swipeStart, swipeEnd);
            ApplyBackspin(transform.position, adjustedTarget, swipeStart, swipeEnd, curvatureFactor);

        }
        else
        {
            Debug.LogWarning("Traiettoria non calcolabile");
            // Se non è possibile calcolare una traiettoria perfetta,
            // proviamo a generare una traiettoria approssimativa "a occhio"
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

    private void ApplyBackspin(Vector3 origin, Vector3 target, Vector2 swipeStart, Vector2 swipeEnd, float curvatureFactor)
    {
        ball.angularVelocity = Vector3.zero;

        float curveThreshold = 0.2f;
        float lateralRatio = Mathf.Clamp((swipeEnd.x - swipeStart.x) / (swipeEnd.y - swipeStart.y + 0.01f), -1f, 1f);
        bool curvedSwipe = Mathf.Abs(lateralRatio) > curveThreshold;
        Vector3 backspin = Vector3.zero;

        if (curvedSwipe)
        {

            /*Vector3 toTarget = target - origin;
            Vector3 directionYZ = new Vector3(0f, toTarget.y, toTarget.z).normalized;
            backspin = -directionYZ * backspinAmount;*/
            
            // Applica rotazione sull’asse X negativa (rotazione in senso orario nel piano YZ)
            backspin = Vector3.left * backspinAmount;

            Debug.Log("Backspin!");
            transform.GetComponent<PlayerManager>().isBackspin(true);
            ball.AddTorque(backspin * curvatureFactor, ForceMode.Impulse);
        }
        else
        {
            transform.GetComponent<PlayerManager>().isBackspin(false);
        }

    }

    private void ApplySpin(Vector3 origin, Vector3 target, Vector2 swipeStart, Vector2 swipeEnd)
    {
        ball.angularVelocity = Vector3.zero;

        Vector3 toTarget = target - origin;
        Vector3 directionXZ = new Vector3(toTarget.x, 0f, toTarget.z).normalized;
        Vector3 lateralDir = Vector3.Cross(Vector3.up, directionXZ);

        float curveThreshold = 0.2f;
        float lateralRatio = Mathf.Clamp((swipeEnd.x - swipeStart.x) / (swipeEnd.y - swipeStart.y + 0.01f), -1f, 1f);
        bool curvedSwipe = Mathf.Abs(lateralRatio) > curveThreshold;

        Vector3 backspin = Vector3.zero;
        Vector3 sidespin = Vector3.zero;

        if (curvedSwipe)
        {
            //backspin = -directionXZ * backspinAmount;
            //sidespin = Vector3.up * lateralRatio * curveSpinAmount;

            Vector3 directionYZ = new Vector3(0f, toTarget.y, toTarget.z).normalized;
            backspin = -directionYZ * backspinAmount;
            sidespin = Vector3.up * lateralRatio * curveSpinAmount;

            // TODO: Implementare una sorta di possibilità extra nell'ottenere un punteggio più alto o comunque premiare il giocatore

        }

        ball.AddTorque(backspin + sidespin, ForceMode.Impulse);
    }

    private bool CalculateArcVelocitySimple(Vector3 target, Vector3 origin, float height, out Vector3 result)
    {
        result = Vector3.zero;
        float gravity = Mathf.Abs(Physics.gravity.y);

        Vector3 displacement = target - origin;
        Vector3 horizontal = new Vector3(displacement.x, 0, displacement.z);

        float h = displacement.y;
        float distance = horizontal.magnitude;

        if (distance < 0.01f) return false;

        float velocityY = Mathf.Sqrt(2 * gravity * height);
        float timeUp = velocityY / gravity;
        float timeDown = Mathf.Sqrt(2 * Mathf.Max(0, height - h) / gravity);
        float totalTime = timeUp + timeDown;

        Vector3 velocityXZ = horizontal / totalTime;
        result = velocityXZ + Vector3.up * velocityY;

        return true;
    }


    private void ApplySpinSimple(Vector3 shotVelocity)
    {
        ball.angularVelocity = Vector3.zero;

        // BACKSPIN verso l’alto rispetto alla direzione di tiro
        Vector3 right = Vector3.Cross(Vector3.up, shotVelocity.normalized);
        Vector3 backspin = right * backspinAmount;

        // CURVA LATERALE opzionale (come nei tiri di calcio)
        Vector3 curveSpin = Vector3.up * curveSpinAmount;

        ball.AddTorque(backspin + curveSpin, ForceMode.Impulse);
    }
}


