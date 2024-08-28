using System.Collections.Generic;
using UnityEngine;

public class PaintBoard : MonoBehaviour
{
    [SerializeField]
    private Line linePrefab;
    [SerializeField]
    private Transform lineParent;
    [SerializeField, Range(1, 1000)]
    private int lineMakeCount = 10;

    private List<Line> linePooling;

    private Camera mainCamera;

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
        bool canDraw = Input.GetMouseButtonDown(0);

        if (!canDraw)
        {
            return;
        }

        float x = Input.mousePosition.x;
        float y = Input.mousePosition.y;

        Vector3 position = new Vector3(x, y, 0f);

        position = mainCamera.ScreenToWorldPoint(position);

        position.z = 0f;

        Line line = GetLine();

        line.transform.position = position;

        line.gameObject.SetActive(true);
    }
}
