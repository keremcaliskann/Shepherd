using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshairs : MonoBehaviour
{
    public LayerMask targetMask;

    public SpriteRenderer dot;
    public Color dotHighlightColor;
    Color originalColor;

    void Start()
    {
        Cursor.visible = false;
        originalColor = dot.color;
    }

    public void DetectTargets(Ray ray)
    {
        if (Physics.Raycast(ray, 100, targetMask))
        {
            dot.color = dotHighlightColor;
        }
        else
        {
            dot.color = originalColor;
        }
    }
}
