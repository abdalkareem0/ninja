using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    private float wallHeight;


    void Start()
    {
        wallHeight = GetComponent<SpriteRenderer>().bounds.size.y;
    }

    // Update is called once per frame
    void Update()
    {
        FixPosition();
    }

    // Check if the wall is out of camera, move to an active position
    private void FixPosition()
    {
        var position = transform.position;

        if (Camera.main.WorldToViewportPoint(position).y < -0.6f)
        {
            position.y += wallHeight * 2.8f;
            transform.position = position;
        }
    }
}
