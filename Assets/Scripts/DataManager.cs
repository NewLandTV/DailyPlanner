using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }

    [Serializable]
    public class Data
    {
        public List<NoteManager.NoteData> noteDatas;
    }

    private Data data = new Data();

    private NoteManager noteManager;
    private WorkManager workManager;

    private string dataPath;

    private void Awake() => Initialize();

    private void Initialize()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);

        dataPath = Path.Join(Application.persistentDataPath, "Notes.json");
    }
    
    private void CheckComponents()
    {
        noteManager = FindObjectOfType<NoteManager>();
        workManager = FindObjectOfType<WorkManager>();
    }

    public void SaveData()
    {
        CheckComponents();

        List<NoteManager.NoteData> noteDatas = noteManager?.Notes;

        if (noteDatas != null)
        {
            data.noteDatas = noteDatas;
        }

        List<Work> workList = workManager?.WorkList;

        if (workList != null)
        {
            NoteManager.NoteData currentNoteData = NoteManager.CurrentNote;
            NoteManager.NoteData note = data.noteDatas?.Find(x => x.date == currentNoteData.date);

            if (note != null)
            {
                note.workDatas = workManager?.WorkDatas;
            }
        }

        WriteToJsonFile();
    }

    private void WriteToJsonFile()
    {
        string jsonData = JsonUtility.ToJson(data, true);

        File.WriteAllText(dataPath, jsonData);
    }

    public void LoadData()
    {
        if (!File.Exists(dataPath))
        {
            return;
        }

        ReadFromJsonFile();

        if (data.noteDatas == null)
        {
            return;
        }

        CheckComponents();

        noteManager?.LoadNoteDatas(data.noteDatas);
    }

    private void ReadFromJsonFile()
    {
        string jsonData = File.ReadAllText(dataPath);

        data = JsonUtility.FromJson<Data>(jsonData);
    }
}
