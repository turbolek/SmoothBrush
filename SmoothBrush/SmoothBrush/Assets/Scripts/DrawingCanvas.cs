using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(RawImage))]
public class DrawingCanvas : MonoBehaviour
{
    // Displays image from renderCamera on its own RawImage component

    [SerializeField]
    private Button clearButton;
    [SerializeField]
    private Camera renderCamera;

    private RawImage rawImage;

    public void Init()
    {
        if (clearButton != null)
        {
            clearButton.onClick.AddListener(delegate
            {
                StartCoroutine(ClearCoroutine());
            });
        }

        rawImage = GetComponent<RawImage>();

        if (renderCamera != null)
        {
            rawImage.texture = renderCamera.targetTexture;
        }

    }
    private IEnumerator ClearCoroutine()
    {
        //Flag camera to clear its render texture and disable clearing in next frame

        renderCamera.clearFlags = CameraClearFlags.SolidColor;
        yield return null;
        renderCamera.clearFlags = CameraClearFlags.Nothing;
    }
}