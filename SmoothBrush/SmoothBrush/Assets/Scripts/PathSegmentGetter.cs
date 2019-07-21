using UnityEngine;
using System.Collections.Generic;

public class PathSegmentGetter
{
    // Calculates points that lay on a given distance on a given path, starting from last previously returned point.

    private List<Vector2> path = new List<Vector2>();
    public GameObject prefab;

    int currentIndex = 0; //stores index of last reached point on the path
    private float totalDistanceToCover = 0f;

    public void SetPath(List<Vector2> path)
    {
        this.path = path;
        currentIndex = 0;
    }

    public Vector2[] GetNextPathSegment(Vector2 segmentStart, float distanceToCover, float sampleSpacing)
    {
        Vector2 currentPositionOnPath = segmentStart;
        List<Vector2> pointsToDraw = new List<Vector2>();

        // utilize leftover distance from previous segment
        totalDistanceToCover += distanceToCover;

        while (totalDistanceToCover >= sampleSpacing)
        {
            if (path.Count > currentIndex + 1)
            {
                float pointDistance = Vector3.Distance(path[currentIndex + 1], currentPositionOnPath);

                if (pointDistance >= sampleSpacing) // if sample can fit between current position and next point on path, add sample between them
                {
                    Vector2 drawDirection = (path[currentIndex + 1] - path[currentIndex]).normalized;
                    currentPositionOnPath += drawDirection * sampleSpacing;
                    pointsToDraw.Add(currentPositionOnPath);
                    totalDistanceToCover -= sampleSpacing;
                }
                else // if sample cannot fit between current position and next point on path, add sample on next point and consider next point pair in next iteration
                {
                    currentIndex++;
                    currentPositionOnPath = path[currentIndex];
                    pointsToDraw.Add(currentPositionOnPath);
                    totalDistanceToCover -= pointDistance;
                }
            }
            else
            {
                totalDistanceToCover = 0f;
            }
        }
        return pointsToDraw.ToArray();
    }
}
