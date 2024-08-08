using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    [Serializable]
    public class Data
    {
        public List<NoteManager.NoteData> noteDatas;
    }

    [SerializeField]
    private NoteManager noteManager;

    private string dataPath;

    private void Awake() => Initialize();

    private void Initialize()
    {
        dataPath = Path.Join(Application.persistentDataPath, "Notes.json");

        LoadData();
    }

    public void SaveData()
    {
        Data data = new Data();
        List<NoteManager.NoteData> noteDatas = noteManager.Notes;

        data.noteDatas = noteDatas;

        string jsonData = JsonUtility.ToJson(data, true);

        File.WriteAllText(dataPath, jsonData);
    }

    private void LoadData()
    {
        if (!File.Exists(dataPath))
        {
            return;
        }

        string jsonData = File.ReadAllText(dataPath);

        Data data = JsonUtility.FromJson<Data>(jsonData);

        if (data.noteDatas != null)
        {
            noteManager.LoadNoteDatas(data.noteDatas);
        }
    }
}
