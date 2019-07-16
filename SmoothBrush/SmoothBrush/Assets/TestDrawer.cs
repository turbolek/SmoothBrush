using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDrawer : MonoBehaviour
{
    List<Vector3> mousePositions;
    public GameObject brush1Prefab;
    public GameObject brush2Prefab;
    public GameObject brush3Prefab;

    private Vector3 lastDrawnPoint;

    [SerializeField]
    private float bufferZoneRadius;

    [SerializeField]
    private float epsilon = .5f;

    private Vector3 anchor;

    Coroutine samplingCoroutine = null;
    Coroutine drawingCoroutine = null;

    void Start()
    {
        mousePositions = new List<Vector3>();
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (samplingCoroutine != null)
            {
                StopCoroutine(samplingCoroutine);
            }

            samplingCoroutine = StartCoroutine(DrawingCoroutine());
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (samplingCoroutine != null)
            {
                StopCoroutine(samplingCoroutine);
            }
            List<Vector3> simplePath = SimplifyPath(mousePositions);

            foreach (Vector3 point in simplePath)
            {
                Instantiate(brush3Prefab, point, Quaternion.identity);
            }
        }

        //    if (mousePositions.Count >= 2 && drawingCoroutine == null)
        //    {
        //        drawingCoroutine = StartCoroutine(DrawSegment());
        //    }
    }

    private IEnumerator DrawingCoroutine()
    {
        mousePositions.Clear();
        while (true)
        {
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPosition.z = 0f;
            mousePositions.Add(mouseWorldPosition);
            Instantiate(brush2Prefab, mouseWorldPosition, Quaternion.identity);

            yield return new WaitForSeconds(0.1f);
        }
    }

    private IEnumerator DrawSegment()
    {
        for (int i = 1; i <= 25; i++)
        {
            Vector3 positionToDraw = iTween.PointOnPath(GetDrawingPath(), (float)i / 25f);
            Instantiate(brush3Prefab, positionToDraw, Quaternion.identity);
            yield return null;
        }
        anchor = mousePositions[0];
        mousePositions.RemoveAt(0);
        drawingCoroutine = null;
    }

    Vector3[] GetDrawingPath()
    {
        Vector3[] path = new Vector3[2];
        path[0] = mousePositions[0];
        path[1] = mousePositions[1];
        return path;
    }

    List<Vector3> SimplifyPath(List<Vector3> path)
    {
        if (path.Count <= 2)
        {
            return path;
        }
        else
        {
            float distance;
            int index;
            Vector3 furthestPoint = GetFurthestMiddlePointOfPath(path, out distance, out index);

            if (distance <= epsilon)
            {
                path.RemoveAt(index);
                return SimplifyPath(path);
            }
            else
            {
                List<Vector3> firstPart = SimplifyPath(path.GetRange(0, index + 1));
                List<Vector3> secondPart = SimplifyPath(path.GetRange(index, path.Count - index));
                secondPart.RemoveAt(0);
                firstPart.AddRange(secondPart);
                return firstPart;
            }
        }
    }

    Vector3 GetFurthestMiddlePointOfPath(List<Vector3> path, out float maxDistance, out int index)
    {
        Ray ray = new Ray(path[0], path[path.Count - 1] - path[0]);
        maxDistance = 0f;
        index = 0;
        Vector3 furthestMiddlePoint = path[0];

        for (int i = 1; i < path.Count - 1; i++)
        {
            Vector3 currentPoint = path[i];
            float currentDistance = Vector3.Cross(ray.direction, currentPoint - ray.origin).magnitude;
            if (currentDistance > maxDistance)
            {
                index = i;
                maxDistance = currentDistance;
                furthestMiddlePoint = currentPoint;
            }
        }
        return furthestMiddlePoint;
    }

    public void DrawRawPositions()
    {
        foreach (Vector3 worldPosition in mousePositions)
        {
            Instantiate(brush1Prefab, worldPosition, Quaternion.identity);
        }
    }

    public void DrawSmoothPositions()
    {
        Vector3[] smoothPositions = MakeSmoothCurve(mousePositions.ToArray(), 3f);
        for (int i = 0; i < smoothPositions.Length; i++)
        {
            Vector3 pos = smoothPositions[i];
            pos.z = -.1f;
            Instantiate(brush3Prefab, pos, Quaternion.identity);
        }
        mousePositions.Clear();
    }

    public static Vector3[] MakeSmoothCurve(Vector3[] arrayToCurve, float smoothness)
    {
        List<Vector3> points;
        List<Vector3> curvedPoints;
        int pointsLength = 0;
        int curvedLength = 0;

        if (smoothness < 1.0f) smoothness = 1.0f;

        pointsLength = arrayToCurve.Length;

        curvedLength = (pointsLength * Mathf.RoundToInt(smoothness)) - 1;
        curvedPoints = new List<Vector3>(curvedLength);

        float t = 0.0f;
        for (int pointInTimeOnCurve = 0; pointInTimeOnCurve < curvedLength + 1; pointInTimeOnCurve++)
        {
            t = Mathf.InverseLerp(0, curvedLength, pointInTimeOnCurve);

            points = new List<Vector3>(arrayToCurve);

            for (int j = pointsLength - 1; j > 0; j--)
            {
                for (int i = 0; i < j; i++)
                {
                    points[i] = (1 - t) * points[i] + t * points[i + 1];
                }
            }

            curvedPoints.Add(points[0]);
        }

        return (curvedPoints.ToArray());
    }


}
