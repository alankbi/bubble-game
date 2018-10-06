using UnityEngine;
using UnityEngine.UI;

public class TextSharpener : MonoBehaviour
{

    public Text[] text;
    private const int Scale = 2;

    // Use this for initialization
    void Start()
    {
        foreach (var t in text)
        {
            SharpenText(t);
        }
    }

    public static void SharpenText(Text t)
    {
        t.fontSize *= Scale;
        t.transform.localScale /= Scale;

        t.horizontalOverflow = HorizontalWrapMode.Overflow;
        t.verticalOverflow = VerticalWrapMode.Overflow;
    }
}
