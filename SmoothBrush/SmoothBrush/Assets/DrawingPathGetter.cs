using UnityEngine;
using System.Collections.Generic;

public class DrawingPathGetter
{
    public GameObject prefab;

    private List<Vector2> path = new List<Vector2>();
    int currentIndex = 0;
    private float totalDistanceToDraw = 0f;

    private class BezierAnchorPoint
    {
        public Vector2 position;
        public Vector2 controlPointBefore;
        public Vector2 controlPointAfter;
    }
    BezierAnchorPoint previousSegmentStart;
    BezierAnchorPoint currentSegmentStart;
    BezierAnchorPoint currentSegmentEnd;

    public void StartNewPath(Vector2 pathStartPosition)
    {
        previousSegmentStart = CreateBezierAnchorPoint(null, pathStartPosition);
        currentSegmentStart = CreateBezierAnchorPoint(null, pathStartPosition);
        currentSegmentEnd = CreateBezierAnchorPoint(null, pathStartPosition);
        currentIndex = 0;
    }

    public void CalculatePath(Vector2 lastDrawnPoint, Vector2 mousePosition)
    {
        path.Clear();
        currentIndex = 0;

        previousSegmentStart = currentSegmentStart;
        currentSegmentStart = CreateBezierAnchorPoint(previousSegmentStart, lastDrawnPoint);
        currentSegmentEnd = CreateBezierAnchorPoint(currentSegmentStart, mousePosition);

        for (float percent = 0f; percent <= 1f; percent += 0.01f)
        {
            Vector2 point = GetCubicBezierPoint(currentSegmentStart.position, currentSegmentStart.controlPointAfter, currentSegmentEnd.controlPointBefore, currentSegmentEnd.position, percent);
            path.Add(point);
            //GameObject.Instantiate(prefab, point, Quaternion.identity);
        }
    }

    public Vector2[] GetNextPathSegment(Vector2 segmentStart, float frameDistanceToDraw, float brushSize)
    {
        Vector2 currentPositionOnPath = segmentStart;
        List<Vector2> pointsToDraw = new List<Vector2>();

        totalDistanceToDraw += frameDistanceToDraw;

        while (totalDistanceToDraw >= brushSize)
        {
            if (path.Count > currentIndex + 1)
            {
                Vector2 drawDirection = (path[currentIndex + 1] - path[currentIndex]).normalized;
                float pointDistance = Vector3.Distance(path[currentIndex + 1], path[currentIndex]);
                if (pointDistance >= brushSize)
                {
                    currentPositionOnPath += drawDirection * brushSize;
                    pointsToDraw.Add(currentPositionOnPath);
                    totalDistanceToDraw -= brushSize;
                }
                else
                {
                    currentIndex++;
                    currentPositionOnPath = path[currentIndex];
                    pointsToDraw.Add(currentPositionOnPath);
                    totalDistanceToDraw -= pointDistance;
                }
            }
            else
            {
                totalDistanceToDraw = 0f;
            }
        }
        return pointsToDraw.ToArray();
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

}
