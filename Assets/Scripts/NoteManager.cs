using System;
using System.Collections.Generic;
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

    private List<NoteData> notes = new List<NoteData>();
    public List<NoteData> Notes => notes;

    public static Action<string> DeleteNote { get; private set; }

    private void Awake() => Initialize();

    private void Start() => DataManager.Instance.LoadData();

    private void Initialize()
    {
        notePooling = new List<GameObject>(noteMakeCount);

        for (int i = 0; i < noteMakeCount; i++)
        {
            MakeNoteUI();
        }

        DeleteNote = DeleteNoteWithDate;
    }

    private bool ParseDate(string dateString, out Date result)
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

    private void LoadNoteScene(NoteData note)
    {
        CurrentNote = note;

        SceneManager.LoadScene(2);
    }

    public void CreateNew()
    {
        if (!ParseDate(dateInputField.text, out Date date))
        {
            return;
        }

        dateInputField.text = string.Empty;

        DateTime nowDateTime = DateTime.Now;

        string now = $"{nowDateTime.Year}.{nowDateTime.Month}.{nowDateTime.Day}";

        if (!ParseDate(now, out Date creationDate))
        {
            return;
        }

        NoteData note = new NoteData(date, creationDate, null);

        notes.Add(note);

        DataManager.Instance.SaveData();

        newNoteInformationBackgroundImage.SetActive(false);

        SetupNoteUI(note, () => LoadNoteScene(note));
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

    private void SetupNoteUI(NoteData note, Action onClick)
    {
        GameObject noteUI = GetNoteUI();

        noteUI.SetActive(true);

        TextMeshProUGUI dateText = noteUI.GetComponentInChildren<TextMeshProUGUI>();

        dateText.text = note.date.String;

        Button button = noteUI.GetComponent<Button>();

        button.onClick.AddListener(() => onClick?.Invoke());
    }

    public void LoadNoteDatas(List<NoteData> noteDatas)
    {
        notes.AddRange(noteDatas);

        for (int i = noteDatas.Count - 1; i >= 0; i--)
        {
            int index = i;

            SetupNoteUI(noteDatas[i], () => LoadNoteScene(noteDatas[index]));
        }
    }

    public void DeleteNoteWithDate(string date)
    {
        if (!ParseDate(date, out Date target))
        {
            return;
        }

        for (int i = notes.Count - 1; i >= 0; i--)
        {
            if (notes[i].date == target)
            {
                notes.RemoveAt(i);

                DataManager.Instance.SaveData();

                return;
            }
        }
    }
}
