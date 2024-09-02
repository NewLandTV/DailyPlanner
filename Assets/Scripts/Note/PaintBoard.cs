using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

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

    private Vector3 lineStartPosition;

    private Camera mainCamera;

    public static Action<List<Vector2>> OnDrawEnd { get; private set; }

    public static bool PointerOn { get; private set; }

    private void Awake() => Initialize();

    private void Update()
    {
        TryDraw();
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

    private void LoadLine(Vector2[] positions)
    {
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

    private void OnLineDrawEnd(List<Vector2> positions)
    {
        positions.Insert(0, lineStartPosition);

        LineData lineData = new LineData(positions.ToArray());

        lineDatas.Add(lineData);

        NoteManager.NoteData noteData = NoteManager.CurrentNote;

        noteData.lineDatas =lineDatas;

        string dateString = noteData.date.String;
        string notePath = DataManager.Instance.GetPath(dateString);

        DataManager.Instance.SaveNoteData(noteData, notePath);
    }

    public void OnPointerEnter(BaseEventData eventData) => PointerOn = true;

    public void OnPointerExit(BaseEventData eventData) => PointerOn = false;
}
