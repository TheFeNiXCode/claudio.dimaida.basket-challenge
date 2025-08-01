using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CPUBattleManager : MonoBehaviour
{

    [Header("References")]
    [SerializeField] private BallShooterCPU cpuShooter;
    [SerializeField] private PositionManager positionManager;
    [SerializeField] private PlayerSettings playerSettings;
    [SerializeField] Cloth Net;

    [Header("Config CPU")]
    public float minShootDelay = 1.5f;
    public float maxShootDelay = 4.0f;  
    public bool canShoot = false;        

    [Header("Precision CPU")]
    public float easySuccessChance = 0.4f;   // 40%
    public float normalSuccessChance = 0.65f;// 65%
    public float hardSuccessChance = 0.85f;  // 85%

    private int cpuPositionIndex = -1;
    private int playerPositionIndex = -1;

    private bool isShoot = false;
    private GameObject currentBall = null;
    protected Transform myTransform { get; private set; }

    [SerializeField] private TMP_Text scoreTextCPU;
    public int scoreCPU { get; private set; }

    public bool isBackboard = false;

    [Header("Score CPU")]
    [SerializeField] public int defaultPointToAssign = 3;
    [SerializeField] public int defaultPointBackboardToAssign = 2;

    protected virtual void Awake()
    {
        myTransform = transform;
    }

    void Start()
    {
        canShoot = false;
        scoreCPU = 0;
        scoreTextCPU.SetText("0");
        StartCoroutine(CPUActionLoop());
    }

    public void OnScore()
    {
        scoreCPU += defaultPointToAssign;
        if(isBackboard) scoreCPU += defaultPointBackboardToAssign;
        scoreTextCPU.SetText(scoreCPU.ToString());
        isBackboard = false;
    }

    private IEnumerator CPUActionLoop()
    {
        while (true)
        {
            yield return new WaitUntil(() => canShoot); // Aspetta finché il gioco è attivo

            float delay = Random.Range(minShootDelay, maxShootDelay);
            yield return new WaitForSeconds(delay);

            if (canShoot && !isShoot)
            {
                ChoosePosition();
                bool willScore = CalculateShotSuccess();
                PerformShot(willScore);
            }
        }
    }

    public void ChoosePosition()
    {
        playerPositionIndex = positionManager.GetIndex();

        int newPos;
        do
        {
            newPos = Random.Range(0, positionManager.PositionsCount());
        } while (newPos == playerPositionIndex);

        cpuPositionIndex = newPos;
        positionManager.SetCPUPosition(this.gameObject, cpuPositionIndex);
    }

    private bool CalculateShotSuccess()
    {
        int difficulty = playerSettings.getGameDifficult();
        float successChance = 0f;

        switch (difficulty)
        {
            case 0: successChance = easySuccessChance; break;
            case 1: successChance = normalSuccessChance; break;
            case 2: successChance = hardSuccessChance; break;
            default: successChance = normalSuccessChance; break;
        }

        return Random.value <= successChance;
    }

    private void PerformShot(bool perfectShot)
    {

        if (currentBall == null)
        {
            GameObject ball = playerSettings.ballSelected();
            if (ball)
            {
                currentBall = Instantiate(ball, myTransform);

                SphereCollider ballCollider = currentBall.GetComponent<SphereCollider>();
                Net.sphereColliders = new ClothSphereColliderPair[]
                {
                    new ClothSphereColliderPair(ballCollider)
                };
            }
        }
        else
        {
            currentBall.transform.SetParent(myTransform);
            currentBall.transform.localPosition = Vector3.zero;
            currentBall.SetActive(true);
        }

        Rigidbody rb = currentBall.GetComponent<Rigidbody>();
        if (rb)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
        int targetMode;
        float force = perfectShot ? 1f: Random.Range(0.7f, 1.3f);
        float precision = perfectShot ? 1f : Random.Range(0.75f, 0.95f);

        if (perfectShot)
        {
            targetMode = (Random.value < 0.65f) ? 1 : 2;
            if (targetMode == 2) isBackboard = true;
            cpuShooter.ShootCPU(currentBall, force, precision, Vector2.zero, Vector2.zero, targetMode);
        }
        else {
            targetMode = 0;
            cpuShooter.ShootCPU(currentBall, force, precision, Vector2.zero, Vector2.zero, targetMode);
        }

        setIsShoot(true);
    }
    public int GetScoreCPU()
    {
        return scoreCPU;
    }

    public void setIsShoot(bool shoot)
    {
        isShoot = shoot;
    }

    public bool getCanShootCPU()
    {
        return canShoot;
    }

    public void setCanShootCPU(bool can)
    {
        if(playerSettings.IsVSCPU()) canShoot = can;
    }
}
