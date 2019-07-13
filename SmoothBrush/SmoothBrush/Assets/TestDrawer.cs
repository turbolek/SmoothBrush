using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDrawer : MonoBehaviour
{
    List<Vector3> mousePositions;
    public GameObject brush1Prefab;
    public GameObject brush2Prefab;

    void Start()
    {
        mousePositions = new List<Vector3>();
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            worldPosition.z = 0f;
            mousePositions.Add(worldPosition);
        }
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
        for (int i = 0; i < mousePositions.Count - 1; i++)
        {
            float percentage = (float)i / mousePositions.Count;
            Debug.Log(percentage.ToString());
            Vector3 smoothPosition = iTween.PointOnPath(mousePositions.ToArray(), percentage);
            smoothPosition.z = -0.1f;
            Instantiate(brush2Prefab, smoothPosition, Quaternion.identity);
        }
    }
}
