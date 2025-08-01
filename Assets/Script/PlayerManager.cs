using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] PlayerSettings settings;
    [SerializeField] Cloth Net;

    [Header("Camera Movement")]
    [SerializeField] GameObject mainCamera;
    [SerializeField] GameObject outTrigger;
    [SerializeField] public bool cameraFollow = true;

    protected Transform myTransform { get; private set; }

    [Header("Input Swipe")]
    public float minSwipeForce = 5f;
    public float maxSwipeForce = 20f;
    public float minPrecision = 0.6f;
    public float maxPrecision = 1.0f;
    public float maxSwipeTime = 3f;
    [Range(0f, 1f)] public float maxSpinCurveFactor = 1.0f;

    private float minSwipeForceShoot = 1f;
    private float maxSwipeForceShoot = 1f;
    private float minPrecisionShoot = 1f;
    private float maxPrecisionShoot = 1f;

    private int target = 0; //0 -> random, 1 -> perfect, 2 -> backboard

    private Vector2 swipeStart;
    private Vector2 swipeEnd;
    private float swipeStartTime;
    private Vector2 swipeDirection;

    private float forceMultiplier = 1.0f;
    private float precision = 1.0f;

    [Header("Controls")]
    [SerializeField] public bool isThisShotABackspin = false;
    [SerializeField] private bool isShotInProgress = false;
    [SerializeField] private HitAreaShoot areaShoot;

    private bool inputEnabled = true;

    [Header("Swipe Trail")]
    [SerializeField] private GameObject swipeTrail;
    private SwipeTrailFade trailFade;
    private bool isSwiping = false;
    private Camera mainCam;

    [Header("Slider Shoot")]
    [SerializeField] private Slider powerSlider;
    [SerializeField] private float maxSwipeDistance = 800f;
    [SerializeField] private float sliderResetSpeed = 200f; // px/sec
    private float currentPower = 0f;
    private Coroutine resetCoroutine;
    public GameObject currentBall = null;
    public Transform fireEffect = null;

    [Header("CPU Battle")]
    [SerializeField] private GameObject CPU;
    [SerializeField] private GameObject CPUTextScore;


    protected virtual void Awake()
    {
        myTransform = transform;
    }
    void Start()
    {
        if (CPU && settings.IsVSCPU())
        {
            CPU.SetActive(true);
            CPUTextScore.SetActive(true);
        }

        if (swipeTrail)
        {
            swipeTrail.SetActive(false);
            trailFade = swipeTrail.GetComponent<SwipeTrailFade>();
        }

        if (powerSlider != null)
            powerSlider.value = 0f;

        mainCam = mainCamera.GetComponent<Camera>();

    }

    private void Update()
    {
        if (inputEnabled)
        {
            ableToShoot();
            UpdateSwipeTrailPosition();
        }
    }

    void HandleSwipe(Vector2 start, Vector2 end)
    {
        Vector2 swipe = end - start;

        if (swipe.magnitude < 30f)
        {
            // Swipe troppo corto, consideralo come click e applica una forza minima
            //swipeDirection = Vector2.up;
            //forceMultiplier = minSwipeForce;
            //precision = minPrecision; // opzionale: tiro impreciso nei semplici click
            //gameObject.GetComponent<BallShooter>().Shoot(ballInstance, forceMultiplier, precision, swipeStart, swipeEnd);
            return;
        }

        if (areaShoot.PerfectShoot(powerSlider.value))
        {
            minSwipeForceShoot = 1f;
            maxSwipeForceShoot = 1f;
            minPrecisionShoot = 1f;
            maxPrecisionShoot = 1f;
            target = 1;
        }
        else if (areaShoot.BackboardShoot(powerSlider.value))
        {
            minSwipeForceShoot = 1f;
            maxSwipeForceShoot = 1f;
            minPrecisionShoot = 1f;
            maxPrecisionShoot = 1f;
            target = 2;
        }
        else
        {
            minSwipeForceShoot = minSwipeForce;
            maxSwipeForceShoot = maxSwipeForce;
            minPrecisionShoot = minPrecision;
            maxPrecisionShoot = maxPrecision;
            target = 0;
        }

        swipeDirection = swipe.normalized;

        float swipeStrength = Mathf.Clamp(swipe.magnitude / Screen.height, 0f, 1f);

        forceMultiplier = Mathf.Lerp(minSwipeForceShoot, maxSwipeForceShoot, swipeStrength);
        precision = Mathf.Lerp(minPrecisionShoot, maxPrecisionShoot, swipeStrength);

        // Curvatura = quanto l’utente si è mosso lateralmente rispetto allo swipe verticale ideale
        float lateralDeviation = Mathf.Abs(swipe.x);
        float verticalMovement = Mathf.Abs(swipe.y);
        float curvatureFactor = Mathf.Clamp01(lateralDeviation / (verticalMovement + 0.01f));

        curvatureFactor *= maxSpinCurveFactor;

        gameObject.GetComponent<BallShooter>().Shoot(instanceBall(), forceMultiplier, precision, swipeStart, swipeEnd, target, curvatureFactor);
    }

    private GameObject instanceBall()
    {
        if (currentBall == null)
        {
            GameObject ball = settings.ballSelected();
            if (ball)
            {
                currentBall = Instantiate(ball, myTransform);

                fireEffect = currentBall.transform.GetChild(0);

                SphereCollider ballCollider = currentBall.GetComponent<SphereCollider>();

                if (Net.sphereColliders == null || Net.sphereColliders.Length == 0)
                {
                    Net.sphereColliders = new ClothSphereColliderPair[]
                    {
                new ClothSphereColliderPair(ballCollider)
                    };
                }
                else
                {
                    var oldArray = Net.sphereColliders;
                    var newArray = new ClothSphereColliderPair[oldArray.Length + 1];
                    oldArray.CopyTo(newArray, 0);
                    newArray[newArray.Length - 1] = new ClothSphereColliderPair(ballCollider);
                    Net.sphereColliders = newArray;
                }
            }
        }
        else
        {
            currentBall.SetActive(true);
            currentBall.transform.SetParent(myTransform);
            currentBall.transform.localPosition = Vector3.zero;
        }

        Rigidbody rb = currentBall.GetComponent<Rigidbody>();
        if (rb)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        if (cameraFollow)
        {
            outTrigger.SetActive(true);
            mainCamera.GetComponent<FollowBall>().FollowTheBall(currentBall);
        }

        isShotInProgress = true;

        return currentBall;
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
                //swipeStartTime = Time.time;

                StartSwipeTrail();
                StartSliderTracking();
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                UpdatePowerSlider(touch.position);
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                swipeEnd = touch.position;
                //float swipeDuration = Time.time - swipeStartTime;
                
                EndSwipeTrail();

                Vector2 swipe = swipeEnd - swipeStart;
                if (swipe.y < 0 || swipe.y < Mathf.Abs(swipe.x))
                {
                    Debug.Log("Swipe ignorato: non è verso l'alto");
                    return;
                }
                HandleSwipe(swipeStart, swipeEnd);
                ResetPowerSlider();
            }
        }
        
#else
        if (Input.GetMouseButtonDown(0))
        {
            swipeStart = Input.mousePosition;
            //swipeStartTime = Time.time;

            StartSwipeTrail();
            StartSliderTracking();
        }
        else if(Input.GetMouseButton(0))
        {
            UpdatePowerSlider(Input.mousePosition);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            swipeEnd = Input.mousePosition;
            //float swipeDuration = Time.time - swipeStartTime;

            EndSwipeTrail();

            Vector2 swipe = swipeEnd - swipeStart;

            if (swipe.y < 0 || swipe.y < Mathf.Abs(swipe.x))
            {
                Debug.Log("Swipe ignorato: non è verso l'alto");
                return;
            }

            HandleSwipe(swipeStart, swipeEnd);
            ResetPowerSlider();

        }
#endif
    }

    private void UpdatePowerSlider(Vector2 currentPos)
    {
        if (powerSlider == null) return;

        float swipeDistance = Mathf.Clamp(currentPos.y - swipeStart.y, 0f, maxSwipeDistance);
        currentPower = (swipeDistance / maxSwipeDistance) * 100f;
        if (currentPower > powerSlider.value)
        {
            powerSlider.value = currentPower;
        }

    }

    private void StartSliderTracking()
    {
        if (resetCoroutine != null)
        {
            StopCoroutine(resetCoroutine);
            resetCoroutine = null;
        }
    }

    private void ResetPowerSlider()
    {
        if (powerSlider == null) return;
        if (resetCoroutine != null) StopCoroutine(resetCoroutine);
        resetCoroutine = StartCoroutine(SmoothResetSlider());
    }

    private IEnumerator SmoothResetSlider()
    {
        while (powerSlider.value > 0f)
        {
            powerSlider.value -= (sliderResetSpeed * Time.deltaTime) * (100f / maxSwipeDistance);
            yield return null;
        }
        powerSlider.value = 0f;
        currentPower = 0f;
    }

    private void StartSwipeTrail()
    {
        if (swipeTrail)
        {
            swipeTrail.SetActive(true);
            isSwiping = true;
        }
    }

    private void EndSwipeTrail()
    {
        isSwiping = false;
        if (trailFade)
            StartCoroutine(trailFade.FadeOut());
    }

    private void UpdateSwipeTrailPosition()
    {
        if (isSwiping && swipeTrail && mainCam)
        {
#if UNITY_ANDROID || UNITY_IOS
            if (Input.touchCount > 0)
            {
                Vector3 pos = Input.GetTouch(0).position;
                pos.z = 1f;
                swipeTrail.transform.position = mainCam.ScreenToWorldPoint(pos);
            }
#else
            Vector3 pos = Input.mousePosition;
            pos.z = 1f;
            swipeTrail.transform.position = mainCam.ScreenToWorldPoint(pos);
#endif
        }
    }


    public void isBackspin(bool isBack)
    {
        isThisShotABackspin = isBack;
    }

    public void fireEnabled() => fireEffect.gameObject.SetActive(true);
    public void fireDisabled() => fireEffect.gameObject.SetActive(false);
    public void hideSwipe() => swipeTrail.SetActive(false);
    public void visibleSwipe() => swipeTrail.SetActive(true);
    public bool InShotInProgress() => isShotInProgress;
    public void OnShotEnded() => isShotInProgress = false;
    public void DisableInput() => inputEnabled = false;
    public void EnableInput() => inputEnabled = true;

    public void EnableInputIfNotShoot() {

        if (!isShotInProgress) inputEnabled = true;
    } 

}
