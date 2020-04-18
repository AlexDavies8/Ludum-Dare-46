using UnityEngine;
using UnityEngine.UI;

public class RenderPanel : MonoBehaviour
{
    public Camera renderCamera;
    public int ppu = 16;

    RawImage image;
    RenderTexture virtualTexture;

    RectTransform myTransform;

    private void Awake()
    {
        myTransform = GetComponent<RectTransform>();
        image = GetComponent<RawImage>();
    }

    private void Update()
    {
        if (virtualTexture == null || virtualTexture.width != myTransform.sizeDelta.x || virtualTexture.height != myTransform.sizeDelta.y)
        {
            CreateVirtualTexture();
            renderCamera.orthographicSize = virtualTexture.height * 0.5f / ppu;
        }
    }

    void CreateVirtualTexture()
    {
        virtualTexture = new RenderTexture((int)myTransform.sizeDelta.x, (int)myTransform.sizeDelta.y, 0);
        virtualTexture.filterMode = FilterMode.Point;

        image.texture = virtualTexture;
        renderCamera.targetTexture = virtualTexture;
    }
}
