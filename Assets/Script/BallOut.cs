using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallOut : MonoBehaviour
{
    [SerializeField] GameObject mainCamera;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball") && other.transform.root.CompareTag("Player"))
        {
            mainCamera.GetComponent<FollowBall>().ReturnToInitialPosition();
            other.gameObject.SetActive(false);
        }
    }
}
