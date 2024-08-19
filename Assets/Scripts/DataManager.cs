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
        public List<string> notePathList;
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

        dataPath = Application.persistentDataPath;
    }
    
    private void CheckComponents()
    {
        noteManager = FindObjectOfType<NoteManager>();
        workManager = FindObjectOfType<WorkManager>();
    }

    public string GetPath(string fileName) => Path.Join(dataPath, fileName);

    public void SaveNewNote(string fileName)
    {
        string path = GetPath(fileName);

        data.notePathList.Add(path);

        path = GetPath("Notes.json");

        WriteToJsonFile(path);
    }

    public void SaveNotePathList(List<string> notePathList)
    {
        data.notePathList = notePathList;

        string path = GetPath("Notes.json");

        WriteToJsonFile(path);
    }

    public void SaveNoteData(NoteManager.NoteData data, string path)
    {
        // TODO ...
    }

    private void WriteToJsonFile(string path)
    {
        string json = JsonUtility.ToJson(data, true);

        File.WriteAllText(path, json);
    }

    public void LoadNotePathList()
    {
        string path = GetPath("Notes.json");
        bool canLoad = ReadFromJsonFile(path) && data.notePathList != null;

        if (!canLoad)
        {
            return;
        }

        CheckComponents();

        noteManager?.LoadNoteList(data.notePathList);
    }

    public void LoadNoteData(string notePath)
    {
        if (!ReadFromJsonFile(notePath))
        {
            return;
        }

        // TODO ...
    }

    private bool ReadFromJsonFile(string path)
    {
        if (!File.Exists(path))
        {
            return false;
        }

        string json = File.ReadAllText(path);

        data = JsonUtility.FromJson<Data>(json);

        return true;
    }
}
