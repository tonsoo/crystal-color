using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public Camera cam;

    public List<Transform> walk_to;
    public float walk_time = 4, dist = .2f;
    float speed = 1;

    public void setCamPosition(Transform pos)
    {
        Vector3 p = pos.position;
        p.z = -10;
        cam.transform.position = p;
    }

    private void Update()
    {
        if(walk_to.Count > 0)
        {
            Vector3 move = walk_to[0].position;
            move.z = -10;

            cam.transform.position = Vector3.MoveTowards(cam.transform.position, move, speed);

            if(Vector2.Distance(cam.transform.position, move) < dist * 2)
            {
                walk_to.RemoveAt(0);

                if(walk_to.Count > 0)
                {
                    move = walk_to[0].position;
                    move.z = -10;
                    speed = Vector2.Distance(cam.transform.position, move) / walk_time;
                }
            }
        }
    }

    public void setTime(float t) { walk_time = t; }

    public void addWalkTo(Transform to)
    {
        speed = Vector2.Distance(cam.transform.position, to.position) / walk_time;
        walk_to.Add(to);
    }
}
