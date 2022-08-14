using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallsController : MonoBehaviour
{
    public static float LeftWallX { get; private set; }
    public static float RightWallX { get; private set; }
    void Awake()
    {
        Camera myCamera = Camera.main;
        var leftWalls = GetChildrenByTag("tag_leftWall");
        var rightWalls = GetChildrenByTag("tag_rightWall");

        float height = 2f * myCamera.orthographicSize;
        float width = height * myCamera.aspect;

        // Adjust position for left walls
        foreach (var wall in leftWalls)
        {
            var position = wall.transform.position;
            position.x = -width / 2 + wall.GetComponent<SpriteRenderer>().bounds.size.x / 2;
            wall.transform.position = position;
            LeftWallX = wall.transform.position.x;
        }

        // Adjust position for right walls
        foreach (var wall in rightWalls)
        {
            var position = wall.transform.position;
            position.x = +width / 2 - wall.GetComponent<SpriteRenderer>().bounds.size.x / 2;
            wall.transform.position = position;
            RightWallX = wall.transform.position.x;
        }


    }

    // Update is called once per frame
    void Update()
    {
    }


    public List<GameObject> GetChildrenByTag(string tag)
    {
        var result = new List<GameObject>();
        foreach (Transform tr in transform)
        {
            if (tr.tag == tag)
            {
                result.Add(tr.GetComponent<Transform>().gameObject);
            }
        }
        return result;
    }
}
