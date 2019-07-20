using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDrawer : MonoBehaviour
{
    private float velocity = 1f;
    private List<Vector3> path = new List<Vector3>();

    private Vector3 currentPosition;

    public GameObject brush1Prefab;
    public GameObject brush2Prefab;
    public GameObject brush3Prefab;

    List<GameObject> drawnPathPoints = new List<GameObject>();
    int currentIndex = 0;

    [SerializeField]
    private float brushSize = .25f;

    Coroutine pathCoroutine = null;
    Coroutine printingCoroutine = null;

    [SerializeField]
    private float pathRefreshTime = .1f;

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
            printingCoroutine = StartCoroutine(PrintCoroutine());
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

        if (!Input.GetMouseButton(0))
        {
            currentPosition = GetMouseWorldPosition();
        }
    }

    private IEnumerator PathCalculationCoroutine()
    {
        Vector3[] pathAnchors = new Vector3[3];
        while (true)
        {
            path.Clear();
            pathAnchors[0] = pathAnchors[1];
            pathAnchors[1] = currentPosition;
            pathAnchors[2] = GetMouseWorldPosition();

            foreach (GameObject pathpoint in drawnPathPoints)
            {
                Destroy(pathpoint.gameObject);
            }
            drawnPathPoints.Clear();

            for (float percent = 0.5f; percent <= 1f; percent += 0.01f)
            {
                Vector3 point = iTween.PointOnPath(pathAnchors, percent);
                path.Add(point);
                drawnPathPoints.Add(Instantiate(brush2Prefab, point, Quaternion.identity));
            }
            Debug.Log(path.Count.ToString());
            yield return new WaitForSeconds(pathRefreshTime);
            currentIndex = 0;
        }
    }

    private IEnumerator PrintCoroutine()
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
                        currentPosition += drawDirection * brushSize;
                        Instantiate(brush3Prefab, currentPosition, Quaternion.identity);
                        distanceToDraw -= brushSize;
                    }
                    else
                    {
                        currentIndex++;
                        currentPosition = path[currentIndex];
                        Instantiate(brush3Prefab, currentPosition, Quaternion.identity);
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

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mouseWorlPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorlPosition.z = 0f;
        return mouseWorlPosition;
    }
}
