using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public abstract class SavableObject : ScriptableObject, ISSavable
{
    public abstract string nameFile { get; }

    protected virtual string SavePath => Path.Combine(Application.persistentDataPath, $"{nameFile}.json");

    private void OnEnable()
    {
        ResetFile();    // Reset dei dati di default
        LoadFile();     // Sovrascrive dai dati salvati (se presenti)
    }

    public void DeleteFile()
    {
        if (File.Exists(SavePath))
        {
            File.Delete(SavePath);
            Debug.Log($"[SaveableScriptableObject] File cancellato: {SavePath}");
        }
    }

    public void LoadFile()
    {
        try
        {
            if (File.Exists(SavePath))
            {
                string json = File.ReadAllText(SavePath);
                JsonUtility.FromJsonOverwrite(json, this);
                Debug.Log($"[SaveableScriptableObject] Caricato da: {SavePath}");
            }
            else
            {
                Debug.LogWarning($"[SaveableScriptableObject] File non trovato, ne creo uno nuovo: {SavePath}");
                SaveFile();
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[SaveableScriptableObject] Errore caricamento: {e.Message}");
        }
    }

    public abstract void ResetFile();

    public void SaveFile()
    {
        try
        {
            string json = JsonUtility.ToJson(this, true);
            File.WriteAllText(SavePath, json);
            Debug.Log($"[SaveableScriptableObject] Salvato in: {SavePath}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[SaveableScriptableObject] Errore salvataggio: {e.Message}");
        }
    }


}
