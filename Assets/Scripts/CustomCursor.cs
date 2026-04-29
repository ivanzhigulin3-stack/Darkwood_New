using UnityEngine;
using UnityEngine.UI;

public class CustomCursor : MonoBehaviour
{
    [SerializeField] private Canvas targetCanvas;
    [SerializeField] private Image cursorImage;

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
    }

    private void Update()
    {
        Vector2 mousePos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            targetCanvas.transform as RectTransform,
            Input.mousePosition,
            targetCanvas.worldCamera,
            out mousePos);

        transform.position = targetCanvas.transform.TransformPoint(mousePos);
    }

    private void OnDestroy()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}
