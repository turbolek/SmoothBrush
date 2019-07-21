using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PrinterController : MonoBehaviour
{
    [SerializeField]
    private Camera mainCamera;
    [SerializeField]
    private DrawingCanvas drawingCanvas;

    [SerializeField]
    private int brushPoolSize = 100;

    [SerializeField]
    private float minVelocity = 1f;
    private float velocity = 2f;
    [SerializeField]
    private float bufferZoneRadius = 1f;

    [SerializeField]
    private GameObject brushPrefab;

    [SerializeField]
    private float brushSize = 1f;

    private float distanceToPrint;
    private Vector2 printingStartPoint;
    private Vector2 lastDrawnPoint;

    private DrawingPathGetter pathGetter;
    private Coroutine pathRefreshCoroutine;
    private float pathRefreshTime = 0.1f;
    private WaitForSeconds pathRefreshWaitForSeconds;

    private BrushPool brushPool;

    private void Awake()
    {
        pathRefreshWaitForSeconds = new WaitForSeconds(pathRefreshTime);

        pathGetter = new DrawingPathGetter();
        brushPool = new BrushPool(brushPrefab, brushPoolSize);
        drawingCanvas.Init();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartPrinting();
        }

        if (Input.GetMouseButton(0) && Vector2.Distance(lastDrawnPoint, GetMouseWorldPosition()) >= bufferZoneRadius)
        {
            velocity = CalculateVelocity();
            distanceToPrint = velocity * Time.deltaTime;
            PrintPathSegment(pathGetter.GetNextPathSegment(lastDrawnPoint, distanceToPrint, brushSize));
        }

        if (Input.GetMouseButtonUp(0))
        {
            StopPrinting();
        }
    }

    private void PrintPathSegment(Vector2[] pathSegment)
    {
        brushPool.ReturnAllInstancesToPool();
        foreach (Vector2 point in pathSegment)
        {
            PrintPoint(point);
        }
    }

    private void PrintPoint(Vector2 point)
    {
        GameObject pointInstance = brushPool.GetInstance();
        pointInstance.transform.localScale = Vector3.one * brushSize;
        pointInstance.transform.position = point;
        lastDrawnPoint = point;
    }

    private IEnumerator PathRefreshCoroutine()
    {
        while (true)
        {
            pathGetter.CalculatePath(lastDrawnPoint, GetMouseWorldPosition());
            yield return new WaitForSeconds(pathRefreshTime / velocity);
        }
    }

    private void StartPrinting()
    {
        printingStartPoint = GetMouseWorldPosition();
        lastDrawnPoint = printingStartPoint;

        if (pathRefreshCoroutine != null)
        {
            StopCoroutine(pathRefreshCoroutine);
        }
        PrintPoint(printingStartPoint);
        pathRefreshCoroutine = StartCoroutine(PathRefreshCoroutine());
    }

    private void StopPrinting()
    {
        brushPool.ReturnAllInstancesToPool();

        if (pathRefreshCoroutine != null)
        {
            StopCoroutine(pathRefreshCoroutine);
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mouseWorlPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mouseWorlPosition.z = 0f;
        return mouseWorlPosition;
    }

    private float CalculateVelocity()
    {
        float mouseDistance = Vector2.Distance(lastDrawnPoint, GetMouseWorldPosition());
        float velocityMultiplier = mouseDistance >= 1f ? mouseDistance : 1f;

        return minVelocity * velocityMultiplier;
    }
}
