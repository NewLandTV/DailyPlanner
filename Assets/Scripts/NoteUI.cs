using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NoteUI : MonoBehaviour
{
    public void Delete()
    {
        TextMeshProUGUI dateText = GetComponentInChildren<TextMeshProUGUI>();

        NoteManager.DeleteNote?.Invoke(dateText.text, DeleteCallback);
    }

    private void DeleteCallback()
    {
        gameObject.SetActive(false);

        Button button = GetComponent<Button>();

        button.onClick.RemoveAllListeners();
    }
}
