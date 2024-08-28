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
    
    private void CheckComponents() => noteManager = FindObjectOfType<NoteManager>();

    public string GetPath(string fileName) => Path.Combine(dataPath, fileName);

    public void SaveNewNote(string fileName)
    {
        string path = GetPath(fileName);

        data.notePathList.Add(path);

        path = GetPath("Notes.json");

        WriteToJsonFile(path, data);
    }

    public void SaveNotePathList(List<string> notePathList)
    {
        data.notePathList = notePathList;

        string path = GetPath("Notes.json");

        WriteToJsonFile(path, data);
    }

    public void SaveNoteData(NoteManager.NoteData data, string path) => WriteToJsonFile(path, data);

    private void WriteToJsonFile(string path, object data)
    {
        string json = JsonUtility.ToJson(data, true);

        File.WriteAllText(path, json);
    }

    public void LoadNotePathList()
    {
        string path = GetPath("Notes.json");
        bool canLoad = ReadFromJsonFile(path, out data) && data.notePathList != null;

        if (!canLoad)
        {
            return;
        }

        CheckComponents();

        noteManager?.LoadNoteList(data.notePathList);
    }

    public bool LoadNoteData(string notePath, out NoteManager.NoteData data)
    {
        bool canLoad = ReadFromJsonFile(notePath, out data);

        if (!canLoad)
        {
            return false;
        }

        return true;
    }

    private bool ReadFromJsonFile<T>(string path, out T result) where T : class
    {
        result = null;

        if (!File.Exists(path))
        {
            return false;
        }

        string json = File.ReadAllText(path);

        result = JsonUtility.FromJson<T>(json);

        return true;
    }

    public bool DeleteFile(string path)
    {
        bool exist = File.Exists(path);

        if (!exist)
        {
            return false;
        }

        File.Delete(path);

        return true;
    }
}
