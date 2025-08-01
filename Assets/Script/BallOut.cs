using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallOut : MonoBehaviour
{
    [SerializeField] GameObject mainCamera;
    [SerializeField] private BackboardManager backboard;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball") && other.transform.root.CompareTag("Player"))
        {
            mainCamera.GetComponent<FollowBall>().ReturnToInitialPosition();
            other.gameObject.SetActive(false);
            backboard.hitBackboard = false;

            other.transform.root.GetComponent<FireballManager>().OnMissedShot();
            other.transform.root.GetComponent<PlayerManager>().OnShotEnded();
        }
        else if (other.CompareTag("Ball") && other.transform.root.CompareTag("CPU"))
        {
            other.gameObject.SetActive(false);

            other.transform.root.GetComponent<CPUBattleManager>().setIsShoot(false);
        }
    }
}
