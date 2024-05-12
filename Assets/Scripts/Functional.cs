using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FunctionPress : MonoBehaviour
{
    SpriteRenderer sr;
    public KeyCode key;
    Color originalColor;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        originalColor = sr.color;
    }

    void Update()
    {
        if (Input.GetKeyDown(key))
        {
            sr.color = new Color(0, 0, 0, 0.5f);
        }
        else if (Input.GetKeyUp(key))
        {
            sr.color = originalColor;
        }
    }
}