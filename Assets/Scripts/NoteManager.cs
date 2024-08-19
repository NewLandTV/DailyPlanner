using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[Serializable]
public struct Date : IEquatable<Date>
{
    public int year;
    public int month;
    public int day;
    public int dayOfWeek;

    public string String => $"{year}.{month:d2}.{day:d2}";

    public Date(int year, int month, int day, int dayOfWeek)
    {
        this.year = year;
        this.month = month;
        this.day = day;
        this.dayOfWeek = dayOfWeek;
    }

    public static bool TryParse(string dateString, out Date result)
    {
        result = new Date();

        bool success = DateTime.TryParse(dateString, out DateTime dateTime);

        if (!success)
        {
            return false;
        }

        result.year = dateTime.Year;
        result.month = dateTime.Month;
        result.day = dateTime.Day;
        result.dayOfWeek = (int)dateTime.DayOfWeek;

        return true;
    }

    public override bool Equals(object obj) => obj is Date date && Equals(date);

    public bool Equals(Date other) => year == other.year && month == other.month && day == other.day;

    public override int GetHashCode() => HashCode.Combine(year, month, day);

    public static bool operator ==(Date a, Date b)
    {
        bool year = a.year == b.year;
        bool month = a.month == b.month;
        bool day = a.day == b.day;
        bool equals = year && month && day;

        return equals;
    }

    public static bool operator !=(Date a, Date b) => !(a == b);
}

public class NoteManager : MonoBehaviour
{
    public static NoteData CurrentNote { get; private set; }

    [SerializeField]
    private GameObject newNoteInformationBackgroundImage;
    [SerializeField]
    private TMP_InputField dateInputField;

    [SerializeField]
    private GameObject notePrefab;
    [SerializeField]
    private Transform noteParent;
    [SerializeField, Range(1, 1000)]
    private int noteMakeCount = 1;

    private List<GameObject> notePooling;

    [Serializable]
    public class NoteData
    {
        public Date date;
        public Date creationDate;

        public List<Work.Data> workDatas;

        public NoteData(Date date, Date creationDate, List<Work.Data> workDatas)
        {
            this.date = date;
            this.creationDate = creationDate;
            this.workDatas = workDatas;
        }
    }

    private List<string> notePathList = new List<string>();

    public static Func<string, bool> DeleteNote { get; private set; }

    private void Awake() => Initialize();

    private void Start() => DataManager.Instance.LoadNotePathList();

    private void Initialize()
    {
        notePooling = new List<GameObject>(noteMakeCount);

        for (int i = 0; i < noteMakeCount; i++)
        {
            MakeNoteUI();
        }

        DeleteNote = DeleteNoteWithDate;
    }

    private void LoadNoteScene(string notePath)
    {
        DataManager.Instance.LoadNoteData(notePath);

        SceneManager.LoadScene(2);
    }

    public void CreateNew()
    {
        if (!Date.TryParse(dateInputField.text, out Date date))
        {
            return;
        }

        dateInputField.text = string.Empty;

        DateTime nowDateTime = DateTime.Now;

        string now = $"{nowDateTime.Year}.{nowDateTime.Month}.{nowDateTime.Day}";

        if (!Date.TryParse(now, out Date creationDate))
        {
            return;
        }

        NoteData note = new NoteData(date, creationDate, null);

        string dateString = date.String;
        string notePath = DataManager.Instance.GetPath(dateString);

        notePathList.Add(notePath);

        DataManager.Instance.SaveNotePathList(notePathList);

        newNoteInformationBackgroundImage.SetActive(false);

        SetupNoteUI(dateString, () => LoadNoteScene(notePath));
    }

    private GameObject MakeNoteUI()
    {
        GameObject instance = Instantiate(notePrefab, noteParent);

        instance.SetActive(false);

        notePooling.Add(instance);

        return instance;
    }

    private GameObject GetNoteUI()
    {
        for (int i = notePooling.Count - 1; i >= 0; i--)
        {
            if (!notePooling[i].activeSelf)
            {
                return notePooling[i];
            }
        }

        return MakeNoteUI();
    }

    private void SetupNoteUI(string dateString, Action onClick)
    {
        GameObject noteUI = GetNoteUI();

        noteUI.SetActive(true);

        TextMeshProUGUI dateText = noteUI.GetComponentInChildren<TextMeshProUGUI>();

        dateText.text = dateString;

        Button button = noteUI.GetComponent<Button>();

        button.onClick.AddListener(() => onClick?.Invoke());
    }

    public void LoadNoteList(List<string> notePathList)
    {
        this.notePathList = notePathList;

        for (int i = notePathList.Count - 1; i >= 0; i--)
        {
            int index = i;
            string dateString = Path.GetFileName(notePathList[i]);

            SetupNoteUI(dateString, () => LoadNoteScene(notePathList[index]));
        }
    }

    public bool DeleteNoteWithDate(string date)
    {
        if (!Date.TryParse(date, out Date target))
        {
            return false;
        }

        for (int i = notePathList.Count - 1; i >= 0; i--)
        {
            string path = Path.GetFileName(notePathList[i]);

            if (!path.Equals(target.String))
            {
                continue;
            }

            notePathList.RemoveAt(i);

            DataManager.Instance.SaveNotePathList(notePathList);

            return true;
        }

        return false;
    }
}
