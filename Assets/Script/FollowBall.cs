using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowBall : MonoBehaviour
{
    [Header("Camera Movement")]
    [SerializeField] private float followDuration = 0.6f;
    [SerializeField, Range(0f, 1f)] private float followPositionWeight = 0.3f; // quanto la camera si muove verso la posizione palla (0=ferma,1=sulla palla)
    [SerializeField, Range(0f, 1f)] private float followRotationWeight = 0.3f; // quanto la camera ruota verso la rotazione palla
    [SerializeField] private float returnDuration = 0.6f;

    [Header("Player Manager")]
    [SerializeField] private PlayerManager playerManager;

    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private bool isFollowing = false;

    private void Start()
    {
        initialPosition = transform.position;
        initialRotation = transform.rotation;

        if (playerManager == null)
        {
            Debug.LogWarning("PlayerManager non assegnato");
        }
    }

    public void UpdateInitialPosition(Vector3 pos, Quaternion rot)
    {
        initialPosition = pos;
        initialRotation = rot;
    }

    private void toggleAnimationMode()
    {
        isFollowing = !isFollowing;
    }

    public void FollowTheBall(GameObject ball)
    {
        if (!isFollowing)
        {
            toggleAnimationMode();
            UpdateInitialPosition(transform.position, transform.rotation);
            StartCoroutine(FollowBallCoroutine(ball));
        }  
    }

    private IEnumerator FollowBallCoroutine(GameObject ball)
    {
        if (playerManager != null)
            playerManager.DisableInput();

        float elapsed = 0f;

        Vector3 previousBallPosition = ball.transform.position;

        while (elapsed < followDuration)
        {
            if (ball == null) break;

            float t = elapsed / followDuration;

            // Posizione: segui parzialmente la traiettoria
            Vector3 targetPosition = Vector3.Lerp(initialPosition, ball.transform.position, followPositionWeight);
            transform.position = Vector3.Lerp(transform.position, targetPosition, t);

            // Calcola direzione di movimento della palla (approssimazione con posizione frame precedente)
            Vector3 velocityDirection = (ball.transform.position - previousBallPosition).normalized;
            previousBallPosition = ball.transform.position;

            // Ignora se la velocità è troppo bassa (palla ferma)
            if (velocityDirection.sqrMagnitude > 0.001f)
            {
                // Proiezione sul piano XZ per eliminare tilt verticale
                Vector3 horizontalDir = new Vector3(velocityDirection.x, 0f, velocityDirection.z).normalized;

                Quaternion targetRotation = Quaternion.LookRotation(horizontalDir != Vector3.zero ? horizontalDir : transform.forward);

                Quaternion weightedRotation = Quaternion.Slerp(initialRotation, targetRotation, followRotationWeight);
                transform.rotation = Quaternion.Slerp(transform.rotation, weightedRotation, t);
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        toggleAnimationMode();
    }

    public void ReturnToInitialPosition()
    {
        if (!isFollowing)
        {
            toggleAnimationMode();
            StartCoroutine(ReturnToInitialPositionCoroutine());
        }
    }

    private IEnumerator ReturnToInitialPositionCoroutine()
    {

        float elapsed = 0f;
        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;

        while (elapsed < returnDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / returnDuration);

            // Easing per evitare scatti improvvisi
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            transform.position = Vector3.Lerp(startPos, initialPosition, smoothT);
            transform.rotation = Quaternion.Slerp(startRot, initialRotation, smoothT);

            yield return null;
        }

        transform.position = initialPosition;
        transform.rotation = initialRotation;

        if (playerManager != null)
            playerManager.EnableInput();

        toggleAnimationMode();
    }
}

