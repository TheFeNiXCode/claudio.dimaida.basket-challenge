using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionManager : MonoBehaviour
{
    [System.Serializable]
    public class PositionPair
    {
        public Transform playerPosition;
        public Transform cameraPosition;
    }

    [Header("Player e Camera")]
    public Transform player;                
    public Transform mainCamera;             
    public float cameraMoveDuration = 1.5f;
    public AnimationCurve cameraMoveCurve = AnimationCurve.EaseInOut(0, 0, 1, 1); // curva per l'interpolazione

    [Header("Positions List")]
    public List<PositionPair> positionPairs;

    private int currentIndex = -1;  
    private bool isMoving = false;  // Evita più movimenti contemporanei

    private PlayerManager playerManager;

    void Start()
    {
        // Controlla che siano presenti almeno delle posizioni valide
        if (positionPairs == null || positionPairs.Count == 0)
        {
            Debug.LogError("Nessuna posizione assegnata!");
            return;
        }

        // Recupera lo script PlayerManager dal player
        playerManager = player.GetComponent<PlayerManager>();
        if (playerManager == null)
        {
            Debug.LogError("PlayerManager non trovato sul player!");
            return;
        }

        // Imposta la posizione iniziale
        currentIndex = 0;
        TeleportPlayer(positionPairs[currentIndex]);
        TeleportCamera(positionPairs[currentIndex]);
    }

    public void OnScore()
    {
        if (isMoving || positionPairs.Count <= 1)
            return;

        // Sceglie una nuova posizione a caso, diversa da quella attuale
        int newIndex;
        do
        {
            newIndex = Random.Range(0, positionPairs.Count);
        } while (newIndex == currentIndex);

        StartCoroutine(MoveCameraToPosition(newIndex));
    }

    IEnumerator MoveCameraToPosition(int newIndex)
    {
        isMoving = true;

        playerManager.DisableInput();

        Vector3 startCamPos = mainCamera.position;
        Quaternion startCamRot = mainCamera.rotation;
        Vector3 endCamPos = positionPairs[newIndex].cameraPosition.position;
        Quaternion endCamRot = positionPairs[newIndex].cameraPosition.rotation;

        float elapsed = 0f;

        TeleportPlayer(positionPairs[newIndex]);

        // Anima la camera lungo la curva di interpolazione
        while (elapsed < cameraMoveDuration)
        {
            float t = elapsed / cameraMoveDuration;
            float curvedT = cameraMoveCurve.Evaluate(t); // Applica la curva per un movimento più fluido

            // Interpolazione posizione e rotazione
            mainCamera.position = Vector3.Lerp(startCamPos, endCamPos, curvedT);
            mainCamera.rotation = Quaternion.Slerp(startCamRot, endCamRot, curvedT);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Imposta esattamente la posizione finale (evita errori di interpolazione)
        mainCamera.position = endCamPos;
        mainCamera.rotation = endCamRot;
        currentIndex = newIndex;
        isMoving = false;

        playerManager.EnableInput();
    }

    private void TeleportPlayer(PositionPair pos)
    {
        player.position = pos.playerPosition.position;
        player.rotation = pos.playerPosition.rotation;
    }

    private void TeleportCamera(PositionPair pos)
    {
        mainCamera.position = pos.cameraPosition.position;
        mainCamera.rotation = pos.cameraPosition.rotation;
    }
}

