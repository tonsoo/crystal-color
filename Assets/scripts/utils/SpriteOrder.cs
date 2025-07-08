using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteOrder : MonoBehaviour
{
    public bool orderOnce = true;
    public int offset = 0;
    public float multi = 10;

    public Renderer sr;

    void Start()
    {
        sr = GetComponent<Renderer>();

        if (orderOnce)
        {
            sr.sortingOrder = 999 - (int)(transform.position.y * multi) + offset;
            Destroy(this);
        }
    }

    void Update()
    {
        sr.sortingOrder = 999 - (int)(transform.position.y * multi) + offset;
    }
}
