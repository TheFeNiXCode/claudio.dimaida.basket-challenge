using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMusic : MonoBehaviour
{
    [SerializeField] PlayerSettings playerSettings;

    void Start()
    {
        AudioManager.Instance.PlayMusic(0, true, 0.7f);
    }

    public void checkIfAudio()
    {
        if (!playerSettings.getAudio())
        {
            Debug.Log("Entro");
            AudioManager.Instance.StopMusic();
        }
        else
        {
            Debug.Log("Entro2");

            AudioManager.Instance.PlayMusic(0, true, 0.7f);
        }
    }
}
