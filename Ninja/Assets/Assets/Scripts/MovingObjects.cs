using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObjects : MonoBehaviour
{
    public const float MOVEMENT_SPEED = 4f;

    // Update is called once per frame
    void Update()
    {
        MoveObjects();
    }

    private void MoveObjects()
    {
        var position = transform.position;
        position.y -= MOVEMENT_SPEED * Time.deltaTime;
        transform.position = position;
    }
}
