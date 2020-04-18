using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableElement : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Rect moveBounds;

    Camera mainCamera;

    bool dragged = false;
    Vector2 offset;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (dragged)
        {
            Vector2 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            Vector2 dragPos = mousePos + offset;
            dragPos.x = Mathf.Clamp(dragPos.x, moveBounds.xMin, moveBounds.xMax);
            dragPos.y = Mathf.Clamp(dragPos.y, moveBounds.yMin, moveBounds.yMax);
            transform.position = dragPos;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Vector2 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        offset = (Vector2)transform.position - mousePos;
        dragged = true;
        transform.SetAsLastSibling();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        dragged = false;
    }

    [ContextMenu("Set Min Bounds")]
    public void SetMinBound()
    {
        moveBounds.min = transform.position;
    }

    [ContextMenu("Set Max Bounds")]
    public void SetMaxBound()
    {
        moveBounds.max = transform.position;
    }
}
