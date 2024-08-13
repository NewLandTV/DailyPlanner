using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WorkManager : MonoBehaviour
{
    [SerializeField]
    private GameObject addWorkSetupBackgroundImage;
    [SerializeField]
    private TMP_InputField titleInputField;

    [SerializeField]
    private Work workPrefab;
    [SerializeField]
    private Transform workParent;
    [SerializeField, Range(1, 100)]
    private int workMakeCount = 10;

    private List<Work> workPooling;
    public List<Work> WorkList { get; private set; }
    public List<Work.Data> WorkDatas
    {
        get
        {
            List<Work.Data> result = new List<Work.Data>();

            for (int i = WorkList.Count - 1; i >= 0; i--)
            {
                result.Add(WorkList[i].RawData);
            }

            return result;
        }
    }

    private void Awake() => Initialize();

    private void Initialize()
    {
        workPooling = new List<Work>(workMakeCount);

        for (int i = 0; i < workMakeCount; i++)
        {
            MakeWork();
        }

        WorkList = new List<Work>();

        LoadWorkDatas(NoteManager.CurrentNote.workDatas);
    }

    private Work MakeWork()
    {
        Work instance = Instantiate(workPrefab, workParent);

        instance.gameObject.SetActive(false);

        workPooling.Add(instance);

        return instance;
    }

    private Work GetWork()
    {
        for (int i = workPooling.Count - 1; i >= 0; i--)
        {
            if (!workPooling[i].gameObject.activeSelf)
            {
                return workPooling[i];
            }
        }

        return MakeWork();
    }

    public void AddWork()
    {
        string title = titleInputField.text;

        Work work = GetWork();
        List<Work.Data> workDatas = NoteManager.CurrentNote.workDatas;

        work.Setup(title);
        workDatas.Add(work.RawData);

        WorkList.Add(work);

        work.gameObject.SetActive(true);

        titleInputField.text = string.Empty;

        DataManager.Instance.SaveData();

        addWorkSetupBackgroundImage.SetActive(false);
    }

    private void AddWork(Work.Data workData)
    {
        Work work = GetWork();

        work.Setup(workData);

        WorkList.Add(work);

        work.gameObject.SetActive(true);
    }

    public void LoadWorkDatas(List<Work.Data> workDatas)
    {
        for (int i = workDatas.Count - 1; i >= 0; i--)
        {
            AddWork(workDatas[i]);
        }
    }
}
