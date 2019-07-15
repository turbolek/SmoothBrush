using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BufferHelper : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        Vector3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        position.z = 0.5f;
        transform.position = position;
    }
}
