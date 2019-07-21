using UnityEngine;
using System.Collections;

public class PrinterController : MonoBehaviour
{
    private float velocity = 2f;
    private float distanceToDraw;
    private float brushSize = 0.16f;
    [SerializeField]
    private GameObject brushPrefab;
    [SerializeField]
    private GameObject brushPrefab2;

    private Vector2 drawingStartPoint;
    private Vector2 lastDrawnPoint;

    private DrawingPathGetter pathGetter;
    private Coroutine pathRefreshCoroutine;
    private float pathRefreshTime = 0.1f;
    private WaitForSeconds pathRefreshWaitForSeconds;

    private Camera renderingCamera;

    private void Awake()
    {
        pathRefreshWaitForSeconds = new WaitForSeconds(pathRefreshTime);
        renderingCamera = Camera.main;

        pathGetter = new DrawingPathGetter();
        pathGetter.prefab = brushPrefab2;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartDrawing();
        }

        if (Input.GetMouseButton(0))
        {
            distanceToDraw = velocity * Time.deltaTime;
            DrawPathSegment(pathGetter.GetNextPathSegment(lastDrawnPoint, distanceToDraw, brushSize));
        }

        if (Input.GetMouseButtonUp(0))
        {
            StopDrawing();
        }
    }

    private void DrawPathSegment(Vector2[] pathSegment)
    {
        foreach (Vector2 point in pathSegment)
        {
            Instantiate(brushPrefab, point, Quaternion.identity);
            lastDrawnPoint = point;
        }
    }

    private IEnumerator PathRefreshCoroutine()
    {
        while (true)
        {
            pathGetter.CalculatePath(lastDrawnPoint, GetMouseWorldPosition());
            yield return pathRefreshWaitForSeconds;
        }
    }

    private void StartDrawing()
    {
        drawingStartPoint = GetMouseWorldPosition();
        lastDrawnPoint = drawingStartPoint;

        if (pathRefreshCoroutine != null)
        {
            StopCoroutine(pathRefreshCoroutine);
        }

        pathRefreshCoroutine = StartCoroutine(PathRefreshCoroutine());
    }

    private void StopDrawing()
    {
        if (pathRefreshCoroutine != null)
        {
            StopCoroutine(pathRefreshCoroutine);
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mouseWorlPosition = renderingCamera.ScreenToWorldPoint(Input.mousePosition);
        mouseWorlPosition.z = 0f;
        return mouseWorlPosition;
    }
}
