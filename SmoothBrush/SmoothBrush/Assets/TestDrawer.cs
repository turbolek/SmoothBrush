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
            transform.position = GetMouseWorldPosition();
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

            for (float percent = 0.5f; percent <= 1f; percent += 0.01f)
            {
                path.Add(iTween.PointOnPath(pathAnchors, percent));
            }
            yield return new WaitForSeconds(pathRefreshTime);
        }
    }

    private IEnumerator PrintCoroutine()
    {
        int currentIndex = 0;
        List<Vector3> pointsToDraw = new List<Vector3>();
        float distanceToDraw = 0f;

        while (true)
        {
            distanceToDraw += velocity * Time.deltaTime;

            while (distanceToDraw > 0f)
            {
                if (path.Count > currentIndex + 1)
                {
                    pointsToDraw.Add(path[currentIndex]);
                    distanceToDraw -= Vector3.Distance(path[currentIndex], path[currentIndex + 1]);
                    currentIndex++;
                }
                else
                {
                    distanceToDraw = 0f;
                }
            }

            foreach (Vector3 point in pointsToDraw)
            {
                Instantiate(brush3Prefab, point, Quaternion.identity);
                currentPosition = point;
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
