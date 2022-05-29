using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    public SpriteRenderer spriteRenderer;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetPosition(int x, int y)
    {
        transform.position = new Vector3(x, y);
    }

    public void SetColor(string color)
    {

        Color newCol;

        if (ColorUtility.TryParseHtmlString(color, out newCol))
        {
            spriteRenderer.color = newCol;
        }
    }
}
