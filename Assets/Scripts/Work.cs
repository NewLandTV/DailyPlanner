using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Work : MonoBehaviour
{
    [Serializable]
    public class Data
    {
        public bool complete;
        public string title;

        public Data(bool complete, string title)
        {
            this.complete = complete;
            this.title = title;
        }
    }

    [SerializeField]
    private Toggle completeToggle;
    [SerializeField]
    private TextMeshProUGUI titleText;

    public bool Complete => completeToggle.isOn;
    public string Title => titleText.text;

    public Data RawData => new Data(Complete, Title);

    public void Setup(string title) => titleText.text = title;

    public void Setup(Data data)
    {
        completeToggle.isOn = data.complete;
        titleText.text = data.title;
    }

    public void SaveStatus() => DataManager.Instance.SaveData();
}
