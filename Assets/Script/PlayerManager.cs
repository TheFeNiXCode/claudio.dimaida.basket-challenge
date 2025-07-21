using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] PlayerSettings settings;

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
        }
#endif
    }

}
