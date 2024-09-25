using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[Serializable]
public class LineData
{
    public Vector2[] positions;

    public LineData(Vector2[] positions)
    {
        this.positions = positions;
    }
}

public class PaintBoard : MonoBehaviour
{
    [SerializeField]
    private Line linePrefab;
    [SerializeField]
    private Transform lineParent;
    [SerializeField, Range(1, 1000)]
    private int lineMakeCount = 10;

    private List<Line> linePooling;
    private List<LineData> lineDatas = new List<LineData>();
    private LineData recentLineData;

    private Vector3 lineStartPosition;

    private Camera mainCamera;

    [SerializeField]
    private Image targetScreenshotAreaImage;
    [SerializeField]
    private GameObject saveToImageButton;

    public static Action<List<Vector2>> OnDrawEnd { get; private set; }

    public static bool PointerOn { get; private set; }

    private void Awake() => Initialize();

    private void Update()
    {
        TryDraw();
        TryUndo();
        TryRedo();
    }

    private void Initialize()
    {
        linePooling = new List<Line>();

        for (int i = 0; i < linePooling.Count; i++)
        {
            MakeLine();
        }

        mainCamera = Camera.main;

        NoteManager.NoteData currentNoteData = NoteManager.CurrentNote;

        bool canLoadLineDatas = currentNoteData.lineDatas != null && currentNoteData.lineDatas.Count > 0;

        if (canLoadLineDatas)
        {
            LoadLineDatas(currentNoteData.lineDatas);
        }

        OnDrawEnd = OnLineDrawEnd;
    }

    private Line MakeLine()
    {
        Line instance = Instantiate(linePrefab, lineParent);

        instance.gameObject.SetActive(false);

        linePooling.Add(instance);

        return instance;
    }

    public Line GetLine()
    {
        for (int i = linePooling.Count - 1; i >= 0; i--)
        {
            if (!linePooling[i].gameObject.activeSelf)
            {
                return linePooling[i];
            }
        }

        return MakeLine();
    }

    private bool DisableRecentLine()
    {
        for (int i = linePooling.Count - 1; i >= 0; i--)
        {
            if (!linePooling[i].gameObject.activeSelf)
            {
                continue;
            }

            if (i < lineDatas.Count)
            {
                recentLineData = lineDatas[i];
            }

            linePooling[i].gameObject.SetActive(false);

            linePooling[i].ClearPositions();
            lineDatas.RemoveAt(i);

            return true;
        }

        return false;
    }

    private void TryDraw()
    {
        bool canDraw = Input.GetMouseButtonDown(0) && PointerOn;

        if (!canDraw)
        {
            return;
        }

        float x = Input.mousePosition.x;
        float y = Input.mousePosition.y;

        lineStartPosition = new Vector3(x, y, 0f);

        lineStartPosition = mainCamera.ScreenToWorldPoint(lineStartPosition);

        lineStartPosition.z = 0f;

        Line line = GetLine();

        line.transform.position = lineStartPosition;

        line.gameObject.SetActive(true);
    }

    private void TryUndo()
    {
        bool canUndo = Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Z);
        
        if (!canUndo)
        {
            return;
        }

        DisableRecentLine();
        SaveNote();
    }

    private void TryRedo()
    {
        bool canRedo = recentLineData != null && Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Y);

        if (!canRedo)
        {
            return;
        }

        lineDatas.Add(recentLineData);

        LoadLine(recentLineData.positions);
        SaveNote();

        recentLineData = null;
    }

    private void LoadLine(Vector2[] positions)
    {
        bool canLoad = positions != null || positions.Length >= 1;

        if (!canLoad)
        {
            return;
        }

        Line line = GetLine();

        line.transform.position = positions[0];

        line.LoadPositions(positions);

        line.gameObject.SetActive(true);
    }

    private void LoadLineDatas(List<LineData> lineDatas)
    {
        this.lineDatas = lineDatas;

        for (int i = 0; i < lineDatas.Count; i++)
        {
            Vector2[] positions = lineDatas[i].positions;

            LoadLine(positions);
        }
    }

    private void SaveNote()
    {
        NoteManager.NoteData noteData = NoteManager.CurrentNote;

        noteData.lineDatas = lineDatas;

        string dateString = noteData.date.String;
        string notePath = DataManager.Instance.GetPath(dateString);

        DataManager.Instance.SaveNoteData(noteData, notePath);
    }

    public void ClearBoard()
    {
        for (int i = linePooling.Count - 1; i >= 0; i--)
        {
            linePooling[i].gameObject.SetActive(false);

            linePooling[i].ClearPositions();
        }

        lineDatas.Clear();

        SaveNote();
    }

    public void SaveToImage() => StartCoroutine(SaveToImageRoutine());

    private IEnumerator SaveToImageRoutine()
    {
        saveToImageButton.SetActive(false);

        yield return new WaitForEndOfFrame();

        // Get calculated canvas' square corvers in the world space.
        Vector3[] corners = new Vector3[4];

        RectTransform rectTransform = targetScreenshotAreaImage.GetComponent<RectTransform>();

        rectTransform.GetWorldCorners(corners);

        for (int i = 0; i < corners.Length; i++)
        {
            corners[i] = mainCamera.WorldToScreenPoint(corners[i]);
        }

        // Calculate x, y, width, height for capture area, rect.
        float x = corners[0].x;
        float y = corners[0].y;
        int width = (int)corners[3].x - (int)corners[0].x;
        int height = (int)corners[1].y - (int)corners[0].y;

        Rect rect = new Rect(x, y, width, height);

        // Take a picture paint board
        Texture2D image = new Texture2D(width, height, TextureFormat.RGB24, false);

        image.ReadPixels(rect, 0, 0);
        image.Apply();

        saveToImageButton.SetActive(true);

        // And save it to png image file.
        byte[] data = image.EncodeToPNG();
        string currentNoteDateString = NoteManager.CurrentNote.date.String;
        string fileName = $"{currentNoteDateString}.png";

        DataManager.Instance.SaveImage(data, fileName);

        if (Application.isPlaying)
        {
            Destroy(image);
        }
    }

    private void OnLineDrawEnd(List<Vector2> positions)
    {
        positions.Insert(0, lineStartPosition);

        LineData lineData = new LineData(positions.ToArray());

        lineDatas.Add(lineData);

        SaveNote();
    }

    public void OnPointerEnter(BaseEventData eventData) => PointerOn = true;

    public void OnPointerExit(BaseEventData eventData) => PointerOn = false;
}
