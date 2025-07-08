using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class PlayerLevel : MonoBehaviour
{
    public float speed;
    public bool canWalk = true;

    public LayerMask layerMask;

    public Transform nextSpot = null;
    public Transform previousSpot = null;
    public float stickDistance = .5f;
    public int direction = 0;

    public AnimationManager anim;

    private void Start()
    {
        Transform start_spot = GameObject.FindWithTag("level_start").transform;
        nextSpot = start_spot;
        transform.position = start_spot.position;
    }

    private void Update()
    {
        if(!canWalk) return;

        if(Input.GetKeyDown(KeyCode.UpArrow))
        {
            RaycastHit2D a = Physics2D.Raycast((Vector2)transform.position, Vector2.up, 50f, layerMask);

            if(a.transform != null)
            {
                nextSpot = a.transform;
                direction = 0;
            }
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            RaycastHit2D a = Physics2D.Raycast((Vector2)transform.position, Vector2.right, 50f, layerMask);

            if (a.transform != null)
            {
                nextSpot = a.transform;
                direction = 1;
            }
        }
        else if(Input.GetKeyDown(KeyCode.DownArrow))
        {
            RaycastHit2D a = Physics2D.Raycast((Vector2)transform.position, Vector2.down, 50f, layerMask);

            if (a.transform != null)
            {
                nextSpot = a.transform;
                direction = 2;
            }
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            RaycastHit2D a = Physics2D.Raycast((Vector2)transform.position, Vector2.left, 50f, layerMask);

            if (a.transform != null)
            {
                nextSpot = a.transform;
                direction = 3;
            }
        }

        if (nextSpot != null)
        {
            if(previousSpot != null)
            {
                previousSpot.gameObject.SetActive(true);
                previousSpot = null;
            }

            Vector2 move = Vector2.zero;
            if (direction == 0)
            {
                move.y = 1;
                anim.changeAnimation("player_back_walk");
            }
            else if (direction == 1)
            {
                move.x = 1;
                anim.changeAnimation("player_right_walk");
            }
            if (direction == 2)
            {
                move.y = -1;
                anim.changeAnimation("player_front_walk");
            }
            else if (direction == 3)
            {
                move.x = -1;
                anim.changeAnimation("player_left_walk");
            }

            transform.position += (Vector3)move * speed * Time.deltaTime;
        }
        else
        {
            if (direction == 0)
            {
                anim.changeAnimation("player_back_idle");
            }
            else if (direction == 1)
            {
                anim.changeAnimation("player_right_idle");
            }
            if (direction == 2)
            {
                anim.changeAnimation("player_front_idle");
            }
            else if (direction == 3)
            {
                anim.changeAnimation("player_left_idle");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.transform == nextSpot)
        {
            transform.position = nextSpot.position;
            previousSpot = nextSpot;
            previousSpot.gameObject.SetActive(false);
            nextSpot = null;
        }
    }

}
