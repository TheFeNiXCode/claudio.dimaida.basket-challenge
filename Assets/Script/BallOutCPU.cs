using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallOutCPU : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball") && other.transform.root.CompareTag("CPU"))
        {
            other.gameObject.SetActive(false);

            other.transform.root.GetComponent<CPUBattleManager>().isBackboard = false;
            other.transform.root.GetComponent<CPUBattleManager>().setIsShoot(false);
        }
    }
}
