using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crystal : MonoBehaviour
{
    public Color crystal_color;

    public SpriteRenderer sr;
    public Light crystal_light;

    public AnimationManager anim;

    public CrystalManager crystal_manager;

    public bool interactable = true;

    public bool played = false;

    void Start()
    {
        crystal_light.color = crystal_color;
    }

    public void changeCrystalColor(Color crystal_color)
    {
        this.crystal_color = crystal_color;
        crystal_light.color = crystal_color;
    }

    public void triggerCrystal()
    {
        if(!interactable || crystal_manager.guessed_order || !played) { return; }

        crystal_manager.checkCrystal(this);
    }

    public void setPlayed(bool p)
    {
        played = p;
    }

    public void beenPlayed()
    {
        played = true;
    }
}
