using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[Serializable]
public class parallex_object
{
    public Transform object_transform;
    public float object_speed = 5;
    public bool random_y = true;
}

public class ParallexController : MonoBehaviour
{
    public float max_x = 0, min_x = 0, max_y, min_y;

    public parallex_object[] parallex_objects;

    public float speed_multiplier = 1;

    void Start()
    {
        for (int i = 0; i < parallex_objects.Length; i++)
        {
            parallex_objects[i].object_transform.transform.Rotate(0, 0, UnityEngine.Random.Range(0, 360));
        }
    }

    void Update()
    {
        for (int i = 0; i < parallex_objects.Length; i++)
        {
            parallex_objects[i].object_transform.transform.position += new Vector3(parallex_objects[i].object_speed, 0) * Time.deltaTime * speed_multiplier;

            if(parallex_objects[i].object_transform.transform.position.x > max_x)
            {
                float y = !parallex_objects[i].random_y ? parallex_objects[i].object_transform.transform.position.y : UnityEngine.Random.Range(min_y, max_y);

                parallex_objects[i].object_transform.transform.position = new Vector3(min_x, y) ;
                parallex_objects[i].object_transform.transform.Rotate(0, 0, UnityEngine.Random.Range(0, 360));
            }
        }
    }
}
