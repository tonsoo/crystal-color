using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float walk_speed = 3, sprint_speed = 5, sprint_anim_speed = 1.25f;
    int direction = 3;
    string anim_dir = "front";
    string end_dir = "";

    public bool canWalk = true, in_cutscene;
    public bool sprinting = false;

    public AnimationManager anim;
    public Rigidbody2D rb;

    public int current_walk_to = 0;
    public List<Transform> walk_to = new List<Transform>();
    public float walk_to_dist = .05f;
    public float interact_dist = 3;
    public Vector2 last_dir;

    public LayerMask interactables;

    public GameObject interactable;

    void Start()
    {
        
    }

    void Update()
    {
        if (CutsceneManager.playingCutscene) { return; }

        if(walk_to.Count > 0)
        {
            float x_to = walk_to[current_walk_to].position.x;
            float y_to = walk_to[current_walk_to].position.y;

            Vector2 move_to = new Vector2(1, 1);

            if (y_to + walk_to_dist > transform.position.y && y_to - walk_to_dist > transform.position.y) { anim_dir = "back"; move_to.x = 0; direction = 0; }
            else if (x_to > transform.position.x && x_to - walk_to_dist > transform.position.x) { anim_dir = "right"; move_to.y = 0; direction = 1; }
            else if (y_to < transform.position.y && y_to - walk_to_dist < transform.position.y) { anim_dir = "front"; move_to.x = 0; move_to.y *= -1; direction = 2; }
            else if (x_to < transform.position.x && x_to - walk_to_dist < transform.position.x) { anim_dir = "left"; move_to.y = 0; move_to.x *= -1; direction = 3; }

            string anim_state_to = (x_to == 0 && y_to == 0) ? "idle" : "walk";

            anim.changeAnimationSpeed(anim_state_to == "walk" ? sprint_anim_speed : 1);

            anim.changeAnimation("player_" + anim_dir + "_" + anim_state_to);

            rb.position += move_to * walk_speed * Time.deltaTime;

            if (Vector2.Distance(transform.position, walk_to[current_walk_to].position) < 0.05f)
            {
                current_walk_to++;

                if(current_walk_to >= walk_to.Count)
                {
                    if (end_dir != "")
                    {
                        anim_dir = end_dir;
                        end_dir = "";
                    }

                    walk_to = new List<Transform>();
                    current_walk_to = 0;

                    if(!in_cutscene)
                        canWalk = true;
                }
            }

            return;
        }

        if (!canWalk || in_cutscene) { anim.changeAnimation("player_" + anim_dir + "_idle"); return; }

        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        Vector2 move = new Vector2(x, y);

        if(move != Vector2.zero)
        {
            last_dir = move;
        }

        RaycastHit2D hit = Physics2D.Raycast((Vector2)transform.position + last_dir / 4, last_dir, interact_dist, interactables);

        if(hit.transform != null)
        {
            interactable = hit.transform.gameObject;
        }
        else
        {
            interactable = null;
        }

        if (y > 0) { anim_dir = "back"; move.x = 0;  }
        else if (x > 0) { anim_dir = "right"; move.y = 0;}
        else if (y < 0) { anim_dir = "front"; move.x = 0; }
        else if (x < 0) { anim_dir = "left"; move.y = 0; }

        if (Input.GetKeyDown(KeyCode.LeftShift)) { sprinting = true; }
        if (Input.GetKeyUp(KeyCode.LeftShift)) { sprinting = false; }

        string anim_state = (x == 0 && y == 0) ? "idle" : "walk";

        anim.changeAnimation("player_" + anim_dir + "_" + anim_state);

        float speed = sprinting ? sprint_speed : walk_speed;

        rb.position += move * speed * Time.deltaTime;
    }

    public void setInCutscene(bool can)
    {
        in_cutscene = can;
    }

    public void setAnimDir(string direction)
    {
        anim_dir = direction;
    }
    public void setEndDir(string direction)
    {
        end_dir = direction;
    }


    public void setWalkTo(List<Transform> to)
    {
        canWalk = false;
        walk_to = to;
    }

    public void addWalkTo(Transform to)
    {
        canWalk = false;
        walk_to.Add(to);
    }

    public void setPosition(Transform position)
    {
        transform.position = position.position;
    }
}
