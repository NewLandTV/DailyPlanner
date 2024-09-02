using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Line : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private Camera mainCamera;

    private bool loaded;

    private void Awake() => Initialize();

    private void Start() => StartCoroutine(Draw());

    private void Initialize()
    {
        lineRenderer = GetComponent<LineRenderer>();
        mainCamera = Camera.main;

        lineRenderer.positionCount = 0;
    }

    private IEnumerator Draw()
    {
        if (loaded)
        {
            yield break;
        }

        List<Vector2> positions = new List<Vector2>();

        while (Input.GetMouseButton(0))
        {
            if (!PaintBoard.PointerOn)
            {
                break;
            }

            int index = lineRenderer.positionCount++;
            float x = Input.mousePosition.x;
            float y = Input.mousePosition.y;

            Vector3 position = new Vector3(x, y, 0f);

            position = mainCamera.ScreenToWorldPoint(position);

            position.z = 0f;

            positions.Add(position);

            lineRenderer.SetPosition(index, position);

            yield return null;
        }

        PaintBoard.OnDrawEnd?.Invoke(positions);
    }

    public void LoadPositions(Vector2[] positions)
    {
        loaded = true;

        List<Vector3> positionList = new List<Vector3>();

        for (int i = 1; i < positions.Length; i++)
        {
            positionList.Add(positions[i]);
        }

        lineRenderer.positionCount = positionList.Count;

        lineRenderer.SetPositions(positionList.ToArray());
    }
}
