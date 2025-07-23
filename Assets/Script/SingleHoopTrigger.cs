using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


[Serializable] public class ScoreEvent : UnityEvent<PlayerManager> { }

public class SingleHoopTrigger : MonoBehaviour
{
    [SerializeField] private float maxPassTime = 1.0f; // Tempo massimo tra Enter ed Exit
    [SerializeField] private float maxUpwardVelocity = -0.1f; // Deve scendere almeno un po'

    [SerializeField] private ScoreEvent onScored;

    private Dictionary<GameObject, float> entryTimes = new Dictionary<GameObject, float>();

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball") && other.transform.root.CompareTag("Player")) {

            Rigidbody rb = other.attachedRigidbody;
            if (rb != null && rb.velocity.y < maxUpwardVelocity)
            {
                // Solo se la palla scende
                if (!entryTimes.ContainsKey(other.gameObject))
                {
                    entryTimes[other.gameObject] = Time.time;
                }
            }
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ball") && other.transform.root.CompareTag("Player")) {

            if (entryTimes.TryGetValue(other.gameObject, out float entryTime))
            {
                if (Time.time - entryTime <= maxPassTime)
                {
                    // Gestione punteggio
                    Debug.Log("Canestro!");
                    onScored.Invoke(other.transform.root.GetComponent<PlayerManager>());
                }
                entryTimes.Remove(other.gameObject);

                StartCoroutine(TimerToDisable(other.gameObject));
            }

        }  
    }

    IEnumerator TimerToDisable(GameObject obj)
    {
        yield return new WaitForSeconds(0.4f);
        obj.SetActive(false);
        // Qui puoi mettere il codice da eseguire dopo 1 secondo
    }

}
