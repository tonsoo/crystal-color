using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneObject : MonoBehaviour
{
    public List<Transform> move_to;

    public float dist = .1f;

    public float speed = 1;
    float time_to_finish = 0;
    float initial_distance;

    public Transform start_pos;

    void Start()
    {
        transform.position = start_pos.position;   
    }

    void Update()
    {
        if(move_to.Count > 0)
        {
            transform.position = Vector3.MoveTowards(transform.position, move_to[0].position, speed * Time.deltaTime);

            if(Vector2.Distance(transform.position, move_to[0].position) < dist * 2)
            {
                move_to.RemoveAt(0);

                if(move_to.Count > 0)
                {
                    initial_distance = Vector2.Distance(transform.position, move_to[0].position);

                    speed = initial_distance / time_to_finish;
                }
            }
        }
    }

    public void setTimeToFinish(float time) { time_to_finish = time; }
    public void addMoveTo(Transform move_to)
    {
        if(this.move_to.Count == 0)
        {
            initial_distance = Vector2.Distance(transform.position, move_to.position);

            speed = initial_distance / time_to_finish;
        }

        this.move_to.Add(move_to);
    }
}
