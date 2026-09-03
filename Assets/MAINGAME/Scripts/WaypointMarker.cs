using UnityEngine;
using UnityEngine.UI;

public class WaypointMarker : MonoBehaviour
{
    public Transform target;

    private RectTransform waypoint_IMG;
    private RectTransform canvasRect;
    private float minX, maxX, minY, maxY;

    private void Start()
    {
        // Auto-assign components so Prefabs don't lose their references
        waypoint_IMG = GetComponent<RectTransform>();

        // Grabs the Canvas that the TaskManager spawned this into
        Canvas parentCanvas = GetComponentInParent<Canvas>();
        if (parentCanvas != null)
        {
            canvasRect = parentCanvas.GetComponent<RectTransform>();
        }
        else
        {
            Debug.LogError("WaypointMarker could not find a parent Canvas!");
            return;
        }

        // Calculate boundaries
        float imageWidth = waypoint_IMG.rect.width / 2;
        float imageHeight = waypoint_IMG.rect.height / 2;

        minX = imageWidth;
        maxX = canvasRect.rect.width - imageWidth;

        minY = imageHeight;
        maxY = canvasRect.rect.height - imageHeight;
    }

    private void Update()
    {
        if (target == null || canvasRect == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 viewportPos = Camera.main.WorldToViewportPoint(target.position);

        Vector2 uiPos = new Vector2(
            viewportPos.x * canvasRect.rect.width,
            viewportPos.y * canvasRect.rect.height
        );

        if (viewportPos.z < 0)
        {
            uiPos.x = canvasRect.rect.width - uiPos.x;
            uiPos.y = canvasRect.rect.height - uiPos.y;

            if (Vector3.Dot((target.position - Camera.main.transform.position), Camera.main.transform.right) < 0)
            {
                uiPos.x = minX;
            }
            else
            {
                uiPos.x = maxX;
            }
        }

        uiPos.x = Mathf.Clamp(uiPos.x, minX, maxX);
        uiPos.y = Mathf.Clamp(uiPos.y, minY, maxY);

        waypoint_IMG.anchoredPosition = uiPos;
    }
}