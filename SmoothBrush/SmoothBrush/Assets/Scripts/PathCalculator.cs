using UnityEngine;
using System.Collections.Generic;

public class PathCalculator
{
    //Calculates Cubic Bezier curve for two given points. Returned curve joints smoothly with previously returned curve. 

    private List<Vector2> path = new List<Vector2>();

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
    }

    public List<Vector2> CalculatePath(Vector2 lastDrawnPoint, Vector2 mousePosition)
    {
        path.Clear();

        //Use start point of previous segment as a reference point of new segment to ensure smooth joint
        previousSegmentStart = currentSegmentStart;
        currentSegmentStart = CreateBezierAnchorPoint(previousSegmentStart, lastDrawnPoint);
        currentSegmentEnd = CreateBezierAnchorPoint(currentSegmentStart, mousePosition);

        for (float percent = 0f; percent <= 1f; percent += 0.01f)
        {
            Vector2 point = GetCubicBezierPoint(currentSegmentStart.position, currentSegmentStart.controlPointAfter, currentSegmentEnd.controlPointBefore, currentSegmentEnd.position, percent);
            path.Add(point);
        }
        return path;
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
            //setup control points symmetrically to ensure smooth joint
            bezierPoint.controlPointBefore = Vector2.Lerp(previousBezierAnchorPoint.controlPointAfter, position, 0.5f);
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
        // uses de Casteljau's algorithm
        Vector2 firstLinearPoint = GetLinearBezierPoint(startPoint, controlPoint, percent);
        Vector2 secondLinearPoint = GetLinearBezierPoint(controlPoint, endPoint, percent);
        return GetLinearBezierPoint(firstLinearPoint, secondLinearPoint, percent);
    }

    private Vector3 GetCubicBezierPoint(Vector2 startPoint, Vector2 controlPoint1, Vector2 controlPoint2, Vector2 endPoint, float percent)
    {
        // uses de Casteljau's algorithm
        Vector2 firstQuadraticPoint = GetQuadraticBezierPoint(startPoint, controlPoint1, controlPoint2, percent);
        Vector2 secondQuadraticPoint = GetQuadraticBezierPoint(controlPoint1, controlPoint2, endPoint, percent);
        return GetLinearBezierPoint(firstQuadraticPoint, secondQuadraticPoint, percent);
    }
}
