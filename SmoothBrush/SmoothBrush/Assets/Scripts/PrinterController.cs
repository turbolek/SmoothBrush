using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PrinterController : MonoBehaviour
{
    // Main class that controls drawing logic

    [Header("App settings")]
    [SerializeField]
    private Camera mainCamera;
    [SerializeField]
    private DrawingCanvas drawingCanvas;
    [SerializeField]
    private int brushPoolSize = 100;

    [Header("Drawing setings")]
    [SerializeField]
    private GameObject brushPrefab;
    [SerializeField]
    private float brushSize = 1f;
    [SerializeField]
    private float minVelocity = 1f;
    private float velocity = 2f;
    [SerializeField]
    private float bufferZoneRadius = 1f;

    private float distanceToPrint;
    private Vector2 printingStartPoint;
    private Vector2 lastPrintedPoint;

    private PathSegmentGetter pathSegmentGetter;
    private PathCalculator pathCalculator;
    private Coroutine pathRefreshCoroutine;
    private float basePathRefreshTime = 0.1f; //Value determined experimentally
    private WaitForSeconds pathRefreshWaitForSeconds;

    private BrushPool brushPool;

    private void Awake()
    {
        pathRefreshWaitForSeconds = new WaitForSeconds(basePathRefreshTime);

        pathSegmentGetter = new PathSegmentGetter();
        brushPool = new BrushPool(brushPrefab, brushPoolSize);
        pathCalculator = new PathCalculator();

        drawingCanvas.Init();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartPrinting();
        }

        if (Input.GetMouseButton(0) && Vector2.Distance(lastPrintedPoint, GetMouseWorldPosition()) >= bufferZoneRadius)
        {
            velocity = CalculateVelocity();
            distanceToPrint = velocity * Time.deltaTime;

            PrintPathSegment(pathSegmentGetter.GetNextPathSegment(lastPrintedPoint, distanceToPrint, brushSize));
        }

        if (Input.GetMouseButtonUp(0))
        {
            StopPrinting();
        }
    }

    private void PrintPathSegment(Vector2[] pathSegment)
    {
        // return drawn points to the pool to reuse them in current segment. Previous points are rendered to a texture so can be safely returned to the bool and remain visible on the screen.
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
        lastPrintedPoint = point;
    }

    private IEnumerator PathRefreshCoroutine()
    {
        while (true)
        {
            // Calculate path for new sets of points and pass it to path segment getter
            pathSegmentGetter.SetPath(pathCalculator.CalculatePath(lastPrintedPoint, GetMouseWorldPosition()));

            // Wait for time dependant on cursor velocity to ensure proper distance between control points. 
            yield return new WaitForSeconds(basePathRefreshTime / velocity);
        }
    }

    private void StartPrinting()
    {
        printingStartPoint = GetMouseWorldPosition();
        lastPrintedPoint = printingStartPoint;

        if (pathRefreshCoroutine != null)
        {
            StopCoroutine(pathRefreshCoroutine);
        }
        //print point at start position to enable single-click point drawing with no need to disable buffer zone
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
        //Convert mouse position from Screen coords to world coords
        return mainCamera.ScreenToWorldPoint(Input.mousePosition);
    }

    private float CalculateVelocity()
    {
        float mouseDistance = Vector2.Distance(lastPrintedPoint, GetMouseWorldPosition());
        float velocityMultiplier = mouseDistance >= 1f ? mouseDistance : 1f;

        return minVelocity * velocityMultiplier;
    }
}
