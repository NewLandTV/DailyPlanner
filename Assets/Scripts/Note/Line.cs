using System.Collections;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Line : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private Camera mainCamera;

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
        while (Input.GetMouseButton(0))
        {
            int index = lineRenderer.positionCount++;
            float x = Input.mousePosition.x;
            float y = Input.mousePosition.y;

            Vector3 position = new Vector3(x, y, 0f);

            position = mainCamera.ScreenToWorldPoint(position);

            position.z = 0f;

            lineRenderer.SetPosition(index, position);

            yield return null;
        }
    }
}
