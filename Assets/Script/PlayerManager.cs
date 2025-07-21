using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] PlayerSettings settings;
    [SerializeField] Cloth Net;

    protected Transform myTransform { get; private set; }

    [Header("Input Swipe")]
    public float minSwipeForce = 5f;
    public float maxSwipeForce = 20f;
    public float minPrecision = 0.6f;
    public float maxPrecision = 1.0f;
    public float maxSwipeTime = 1f;
    [Range(0f, 1f)]
    public float maxSpinCurveFactor = 1.0f;

    private Vector2 swipeStart;
    private Vector2 swipeEnd;
    private float swipeStartTime;
    private Vector2 swipeDirection;

    private float forceMultiplier = 1.0f;
    private float precision = 1.0f;

    protected virtual void Awake()
    {
        myTransform = transform;
    }
    void Start()
    {

    }

    private void Update()
    {
        ableToShoot();

    }

    void HandleSwipe(Vector2 start, Vector2 end, float duration, GameObject ballInstance)
    {
        if (duration > maxSwipeTime) return;

        Vector2 swipe = end - start;

        if (swipe.magnitude < 50f)
        {
            // Swipe troppo corto, consideralo come click e applica una forza minima
            swipeDirection = Vector2.up;
            forceMultiplier = minSwipeForce;
            precision = minPrecision; // opzionale: tiro impreciso nei semplici click
            gameObject.GetComponent<BallShooter>().Shoot(ballInstance, forceMultiplier, precision, swipeStart, swipeEnd);
            return;
        }

        swipeDirection = swipe.normalized;

        float swipeStrength = Mathf.Clamp(swipe.magnitude / Screen.height, 0f, 1f);

        forceMultiplier = Mathf.Lerp(minSwipeForce, maxSwipeForce, swipeStrength);
        precision = Mathf.Lerp(minPrecision, maxPrecision, swipeStrength);

        // Curvatura = quanto l’utente si è mosso lateralmente rispetto allo swipe verticale ideale
        float lateralDeviation = Mathf.Abs(swipe.x);
        float verticalMovement = Mathf.Abs(swipe.y);
        float curvatureFactor = Mathf.Clamp01(lateralDeviation / (verticalMovement + 0.01f));

        curvatureFactor *= maxSpinCurveFactor;

        gameObject.GetComponent<BallShooter>().Shoot(ballInstance, forceMultiplier, precision, swipeStart, swipeEnd, curvatureFactor);
    }


    private GameObject instanceBall()
    {
        GameObject ball = settings.ballSelected();
        if (ball)
        {
            GameObject ballInstance = Instantiate(ball, myTransform);
            ballInstance.transform.localPosition = Vector3.zero;

            SphereCollider ballCollider = ballInstance.GetComponent<SphereCollider>();
            Net.sphereColliders = new ClothSphereColliderPair[]
            {
                new ClothSphereColliderPair(ballCollider)
            };

            return ballInstance;
        }
        return null;
    }

    private void ableToShoot()
    {
#if UNITY_ANDROID || UNITY_IOS

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                swipeStart = touch.position;
                swipeStartTime = Time.time;
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                swipeEnd = touch.position;
                float swipeDuration = Time.time - swipeStartTime;
                
                Vector2 swipe = swipeEnd - swipeStart;
                if (swipe.y < 0 || swipe.y < Mathf.Abs(swipe.x))
                {
                    Debug.Log("Swipe ignorato: non è verso l'alto");
                    return;
                }
                HandleSwipe(swipeStart, swipeEnd, swipeDuration, instanceBall());
            }
        }
        
#else
        if (Input.GetMouseButtonDown(0))
        {
            swipeStart = Input.mousePosition;
            swipeStartTime = Time.time;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            swipeEnd = Input.mousePosition;
            float swipeDuration = Time.time - swipeStartTime;

            Vector2 swipe = swipeEnd - swipeStart;

            if (swipe.y < 0 || swipe.y < Mathf.Abs(swipe.x))
            {
                Debug.Log("Swipe ignorato: non è verso l'alto");
                return;
            }

            HandleSwipe(swipeStart, swipeEnd, swipeDuration, instanceBall());

        }
#endif
    }

}
