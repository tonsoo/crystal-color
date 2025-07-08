using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class npc_default_path
{
    public bool random = false;
    public bool iterating_goes_next = false;

    public bool random_iterations = false;
    public int maxIterations = 5;
    public int iterations_to_next = 1;
    public int next_path = -1;

    public float path_speed = 2.5f;
    public List<npc_path> path;
}

[Serializable]
public class npc_path
{
    public Transform path;

    public string end_look_direction = "";

    public bool random_delay = false;
    public float reach_delay = 5, max_delay = 0, min_delay = 0;
}

public class NPCScript : MonoBehaviour
{
    public string character_name = "gatuno";
    string anim_direction = "front", end_direction = "";

    public float walk_speed = 3, sprint_speed = 5, distance = .2f;

    public List<Transform> walk_to = new List<Transform>();

    public AnimationManager anim;

    public int current_path = 0, current_walk = 0, iterations = 0;
    public List<npc_default_path> npc_path;

    public bool follow_npc_path = false;

    void Start()
    {
        
    }

    void Update()
    {
        if (CutsceneManager.playingCutscene) { return; }

        if(walk_to.Count > 0){
            Vector3 move = Vector3.zero;

            if (transform.position.y + distance < walk_to[0].position.y && transform.position.y - distance < walk_to[0].position.y)
            {
                move.y = 1;
                anim_direction = "back";
            }
            else if (transform.position.x + distance < walk_to[0].position.x && transform.position.x - distance < walk_to[0].position.x)
            {
                move.x = 1;
                anim_direction = "right";
            }
            else if (transform.position.y + distance > walk_to[0].position.y && transform.position.y - distance > walk_to[0].position.y)
            {
                move.y = -1;
                anim_direction = "front";
            }
            else if (transform.position.x + distance > walk_to[0].position.x && transform.position.x - distance > walk_to[0].position.x)
            {
                move.x = -1;
                anim_direction = "left";
            }

            string anim_state = move == Vector3.zero ? "idle" : "walk";

            anim.changeAnimation(character_name + "_" + anim_direction + "_" + anim_state);

            float speed = walk_speed;

            if (follow_npc_path)
            {
                speed = npc_path[current_path].path[current_walk].path.position == walk_to[0].position ? npc_path[current_path].path_speed : walk_speed;
            }

            transform.position += move * speed * Time.deltaTime;

            if (Vector2.Distance(transform.position, walk_to[0].position) < distance * 2)
            {
                if(npc_path.Count > 0 && follow_npc_path)
                {
                    if(npc_path[current_path].path[current_walk].path.position == walk_to[0].position)
                    {
                        if(npc_path[current_path].path[current_walk].end_look_direction != "")
                        {
                            Debug.Log("a");
                            anim_direction = npc_path[current_path].path[current_walk].end_look_direction;
                        }
                        StartCoroutine(waitNpcPathDelay());
                    }
                }
                else
                {
                    if(end_direction != "" && walk_to.Count == 1)
                    {
                        anim_direction = end_direction;
                        end_direction = "";
                    }
                }

                walk_to.RemoveAt(0);
            }

            return;
        }

        anim.changeAnimation(character_name + "_" + anim_direction + "_idle");

        if(npc_path.Count > current_path && current_path >= 0 && follow_npc_path)
        {
            int index = current_walk;

            if (npc_path[current_path].random)
            {
                while(index == current_walk)
                {
                    index = UnityEngine.Random.Range(0, npc_path[current_path].path.Count);
                }

                current_walk = index;
            }

            addWalkTo(npc_path[current_path].path[index].path);

            follow_npc_path = false;
        }
    }

    IEnumerator waitNpcPathDelay()
    {
        bool walk = true;
        float delay = npc_path[current_path].path[current_walk].reach_delay;

        if (npc_path[current_path].path[current_walk].random_delay)
        {
            delay = UnityEngine.Random.Range(npc_path[current_path].path[current_walk].min_delay, npc_path[current_path].path[current_walk].max_delay);
        }

        yield return new WaitForSeconds(delay);

        if (!npc_path[current_path].random)
        {
            current_walk++;

            if(current_walk > npc_path[current_path].path.Count - 1)
            {
                current_walk = 0;
            }
        }

        if (npc_path[current_path].iterating_goes_next)
        {
            iterations++;

            if(iterations >= npc_path[current_path].iterations_to_next && npc_path[current_path].iterations_to_next > 0)
            {
                iterations = 0;

                if (npc_path[current_path].next_path < 0)
                {
                    current_path++;
                }
                else
                {
                    current_path = npc_path[current_path].next_path;
                }

                current_walk = 0;

                if (current_path > npc_path.Count - 1)
                {
                    walk = false;
                }
            }
            else if(npc_path[current_path].iterations_to_next == 0)
            {
                if(iterations > npc_path[current_path].path.Count - 1)
                {
                    walk_to = new List<Transform>();
                    iterations = 0;

                    if (npc_path[current_path].next_path < 0)
                    {
                        current_path++;
                    }
                    else
                    {
                        current_path = npc_path[current_path].next_path;
                    }

                    current_walk = 0;

                    if (current_path > npc_path.Count - 1)
                    {
                        walk = false;
                    }
                }
            }
        }

        yield return null;

        follow_npc_path = walk;
    }

    public void unFollowNpcPath()
    {
        StopAllCoroutines();
        follow_npc_path = false;
    }

    public void followNpcPath(int start)
    {
        current_walk = Mathf.Clamp(start, 0, npc_path[current_path].path.Count - 1);
        if (npc_path[current_path].random_iterations && npc_path[current_path].iterating_goes_next)
        {
            npc_path[current_path].iterations_to_next = UnityEngine.Random.Range(1, npc_path[current_path].maxIterations);
        }
        follow_npc_path = true;
    }

    public void setEndDirection(string direction)
    {
        end_direction = direction;
    }

    public void addWalkTo(Transform to)
    {
        walk_to.Add(to);
    }

    public void setPosition(Transform newPos)
    {
        transform.position = newPos.position;
    }
}
