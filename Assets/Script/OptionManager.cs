using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] PlayerSettings playerSettings;
    [SerializeField] private Slider difficultySlider;
    [SerializeField] private Slider sensivitySlider;
    [SerializeField] private Toggle audioToggle;


    public void setNewDifficulty()
    {
        playerSettings.setGameDifficult((int) difficultySlider.value);
    }

    public void setNewSensivity()
    {
        playerSettings.setGameSensivity((int) sensivitySlider.value);
    }

    public void setNewAudio()
    {
        playerSettings.setAudio(audioToggle.isOn);
    }

    private void Start()
    {
        difficultySlider.SetValueWithoutNotify(playerSettings.getGameDifficult());
        sensivitySlider.SetValueWithoutNotify(playerSettings.getSensivity());
        audioToggle.SetIsOnWithoutNotify(playerSettings.getAudio());
    }
}
