using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NoteUI : MonoBehaviour
{
    public void Delete()
    {
        gameObject.SetActive(false);

        Button button = GetComponent<Button>();

        button.onClick.RemoveAllListeners();

        TextMeshProUGUI dateText = GetComponentInChildren<TextMeshProUGUI>();

        NoteManager.DeleteNote?.Invoke(dateText.text);
    }
}
