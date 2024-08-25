using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Note : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI titleText;
    [SerializeField]
    private Image dayOfWeekImage;
    [SerializeField]
    private TextMeshProUGUI creationDateText;
    [SerializeField]
    private Image creationDateDayOfWeekImage;
    [SerializeField]
    private Sprite[] dayOfWeekSprites;

    private void Awake() => Initialize();

    private void Initialize()
    {
        NoteManager.NoteData currentNoteData = NoteManager.CurrentNote;

        if (currentNoteData == null)
        {
#if UNITY_EDITOR
            Debug.Log("Invalid note data!");
#endif
            return;
        }

        Date date = currentNoteData.date;

        titleText.text = date.String;
        dayOfWeekImage.sprite = dayOfWeekSprites[date.dayOfWeek];

        Date creationDate = currentNoteData.creationDate;

        creationDateText.text = creationDate.String;
        creationDateDayOfWeekImage.sprite = dayOfWeekSprites[creationDate.dayOfWeek];
    }

    public void GoMainPage() => SceneManager.LoadScene(1);
}
