using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DrawingCanvas : MonoBehaviour
{
    [SerializeField]
    private Button clearButton;
    [SerializeField]
    private Camera renderCamera;

    public void Init()
    {
        if (clearButton != null)
        {
            clearButton.onClick.AddListener(delegate
            {
                StartCoroutine(ClearCoroutine());
            });
        }


    }
    private IEnumerator ClearCoroutine()
    {
        renderCamera.clearFlags = CameraClearFlags.SolidColor;
        yield return null;
        renderCamera.clearFlags = CameraClearFlags.Nothing;
    }
}