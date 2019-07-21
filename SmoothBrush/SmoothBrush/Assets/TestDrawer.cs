using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDrawer : MonoBehaviour
{
    private Printer printer;

    public class BezierAnchorPoint
    {
        public Vector2 position;
        public Vector2 controlPointBefore;
        public Vector2 controlPointAfter;
    }

    [SerializeField]
    private float velocityFactor = 1f;

    private List<Vector3> path = new List<Vector3>();


    public GameObject brush1Prefab;
    public GameObject brush2Prefab;
    public GameObject brush3Prefab;

    List<GameObject> drawnPathPoints = new List<GameObject>();
    List<GameObject> drawnAnchors = new List<GameObject>();
    List<GameObject> drawnPoint = new List<GameObject>();
    int currentIndex = 0;

    Vector3 lastFrameMousePosition;

    [SerializeField]
    private float brushSize = .25f;

    Coroutine pathCoroutine = null;
    Coroutine printingCoroutine = null;

    [SerializeField]
    private float pathRefreshTime = .5f;

    private void Awake()
    {
        printer = new Printer();
        printer.SetBrushSize(brushSize);
        printer.SetBrushPrefab(brush3Prefab);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (pathCoroutine != null)
            {
                StopCoroutine(pathCoroutine);
            }
            if (printingCoroutine != null)
            {
                StopCoroutine(printingCoroutine);
            }

            pathCoroutine = StartCoroutine(PathCalculationCoroutine());
            printingCoroutine = StartCoroutine(printer.PrintCoroutine());
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (pathCoroutine != null)
            {
                StopCoroutine(pathCoroutine);
            }
            if (printingCoroutine != null)
            {
                StopCoroutine(printingCoroutine);
            }
        }

        //if (!Input.GetMouseButton(0))
        //{
        //    printer.CurrentPosition = GetMouseWorldPosition();
        //}

        printer.SetVelocity(CalculatePrinterVelocity());
    }

    private IEnumerator PathCalculationCoroutine()
    {
        BezierAnchorPoint[] pathAnchors = new BezierAnchorPoint[3];

        pathAnchors[0] = CreateBezierAnchorPoint(null, GetMouseWorldPosition());
        pathAnchors[1] = CreateBezierAnchorPoint(null, GetMouseWorldPosition());
        pathAnchors[2] = CreateBezierAnchorPoint(null, GetMouseWorldPosition());

        while (true)
        {
            path.Clear();
            pathAnchors[0] = pathAnchors[1];
            pathAnchors[1] = CreateBezierAnchorPoint(pathAnchors[0], printer.CurrentPosition);
            pathAnchors[2] = CreateBezierAnchorPoint(pathAnchors[1], GetMouseWorldPosition());

            foreach (GameObject drawnAnchor in drawnAnchors)
            {
                Destroy(drawnAnchor.gameObject);
            }
            drawnAnchors.Clear();

            foreach (BezierAnchorPoint pathAnchor in pathAnchors)
            {
                // drawnAnchors.Add(Instantiate(brush1Prefab, pathAnchor.position, Quaternion.identity));
            }

            foreach (GameObject pathpoint in drawnPathPoints)
            {
                Destroy(pathpoint.gameObject);
            }
            drawnPathPoints.Clear();

            for (float percent = 0f; percent <= 1f; percent += 0.01f)
            {
                Vector2 point = GetCubicBezierPoint(pathAnchors[1].position, pathAnchors[1].controlPointAfter, pathAnchors[2].controlPointBefore, pathAnchors[2].position, percent);
                path.Add(point);
                //drawnPathPoints.Add(Instantiate(brush2Prefab, point, Quaternion.identity));
            }
            yield return new WaitForSeconds(pathRefreshTime);
            currentIndex = 0;
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mouseWorlPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorlPosition.z = 0f;
        return mouseWorlPosition;
    }

    private Vector2 GetLinearBezierPoint(Vector2 startPoint, Vector2 endPoint, float percent)
    {
        return Vector2.Lerp(startPoint, endPoint, percent);
    }

    private Vector2 GetQuadraticBezierPoint(Vector2 startPoint, Vector2 controlPoint, Vector2 endPoint, float percent)
    {
        Vector2 firstLinearPoint = GetLinearBezierPoint(startPoint, controlPoint, percent);
        Vector2 secondLinearPoint = GetLinearBezierPoint(controlPoint, endPoint, percent);
        return GetLinearBezierPoint(firstLinearPoint, secondLinearPoint, percent);
    }

    private Vector3 GetCubicBezierPoint(Vector2 startPoint, Vector2 controlPoint1, Vector2 controlPoint2, Vector2 endPoint, float percent)
    {
        Vector2 firstQuadraticPoint = GetQuadraticBezierPoint(startPoint, controlPoint1, controlPoint2, percent);
        Vector2 secondQuadraticPoint = GetQuadraticBezierPoint(controlPoint1, controlPoint2, endPoint, percent);
        return GetLinearBezierPoint(firstQuadraticPoint, secondQuadraticPoint, percent);
    }

    private BezierAnchorPoint CreateBezierAnchorPoint(BezierAnchorPoint previousBezierAnchorPoint, Vector2 position)
    {
        BezierAnchorPoint bezierPoint = new BezierAnchorPoint();

        bezierPoint.position = position;

        if (previousBezierAnchorPoint == null)
        {
            bezierPoint.controlPointBefore = position;
            bezierPoint.controlPointAfter = position;
        }
        else
        {
            bezierPoint.controlPointBefore = Vector2.Lerp(previousBezierAnchorPoint.position, position, 0.5f);
            bezierPoint.controlPointAfter = position + (position - bezierPoint.controlPointBefore);
        }

        return bezierPoint;
    }

    public void Clear()
    {
        foreach (GameObject point in drawnPoint)
        {
            Destroy(point.gameObject);
        }
        drawnPoint.Clear();
    }

    private float CalculatePrinterVelocity()
    {
        float distance = Vector3.Distance(printer.CurrentPosition, GetMouseWorldPosition());
        return Mathf.Clamp(distance * velocityFactor, 1f, 100f);
    }
}
