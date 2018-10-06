using UnityEngine;

public class BackgroundResizer : MonoBehaviour
{

    public Transform canvas;
    public GameObject background;
    private Rect canvasDimensions;

    // Use this for initialization
    void Start()
    {
        canvasDimensions = canvas.GetComponent<RectTransform>().rect;
        var backgroundDimensions = background.GetComponent<SpriteRenderer>().sprite.bounds.size;
        background.transform.localScale = new Vector2(canvasDimensions.width / (1 * backgroundDimensions.x), canvasDimensions.height / (1 * backgroundDimensions.y));
    }
}
