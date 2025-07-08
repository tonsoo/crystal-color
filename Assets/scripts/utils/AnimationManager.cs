using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    public string curAnimation = "";

    public Animator anim;

    public void changeAnimation(string newAnimation)
    {
        if (newAnimation == curAnimation)
            return;

        curAnimation = newAnimation;
        anim.Play(newAnimation);
    }

    public void changeAnimationSpeed(float new_speed)
    {
        anim.speed = new_speed;
    }
}
