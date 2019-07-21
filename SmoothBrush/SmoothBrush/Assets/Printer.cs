using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Printer
{

    List<GameObject> drawnPoint = new List<GameObject>();
    int currentIndex = 0;
    private float velocity = 1f;
    private float brushSize = .25f;
    private GameObject brushPrefab;

    private List<Vector3> path = new List<Vector3>();

    public Vector3 CurrentPosition { get; private set; }

    public void SetVelocity(float velocity)
    {
        this.velocity = velocity;
    }

    public void SetBrushSize(float brushSize)
    {
        this.brushSize = brushSize;
    }

    public void SetBrushPrefab(GameObject brushPrefab)
    {
        this.brushPrefab = brushPrefab;
    }

    public IEnumerator PrintCoroutine()
    {
        currentIndex = 0;
        List<Vector3> pointsToDraw = new List<Vector3>();
        float distanceToDraw = 0f;

        while (true)
        {
            distanceToDraw += velocity * Time.deltaTime;

            while (distanceToDraw >= brushSize)
            {
                if (path.Count > currentIndex + 1)
                {
                    Vector3 drawDirection = (path[currentIndex + 1] - path[currentIndex]).normalized;
                    float pointDistance = Vector3.Distance(path[currentIndex + 1], path[currentIndex]);
                    if (pointDistance >= brushSize)
                    {
                        CurrentPosition += drawDirection * brushSize;
                        drawnPoint.Add(GameObject.Instantiate(brushPrefab, CurrentPosition, Quaternion.identity));
                        distanceToDraw -= brushSize;
                    }
                    else
                    {
                        currentIndex++;
                        CurrentPosition = path[currentIndex];
                        drawnPoint.Add(GameObject.Instantiate(brushPrefab, CurrentPosition, Quaternion.identity));
                        distanceToDraw -= pointDistance;
                    }
                }
                else
                {
                    distanceToDraw = 0f;
                }
            }
            yield return null;
        }
    }
}
