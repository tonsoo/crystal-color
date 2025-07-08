using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CrystalManager : MonoBehaviour
{
    public List<Crystal> crystals;
    public List<Sprite> crystal_sprites;

    public int iterations = 2, cur_trigger = 0;
    public bool can_set_order = true;
    public List<int> crystal_order;
    public float crystal_blink_delay = .5f;

    public bool new_order_wrong = false, play_on_start, guessed_order = false;

    public InteractTrigger trigger;

    public AudioSource source;
    public AudioClip crystal_guess;
    public AudioClip crystal_wrong;

    public bool guessed_wrong = false;

    public int firstTry = 0, errors = 0, tries = 0;

    EstatisticasManager em;

    void Start()
    {
        em = FindObjectOfType<EstatisticasManager>();

        for (int i = 0; i < crystals.Count; i++)
        {
            crystals[i].crystal_manager = this;
            crystals[i].sr.sprite = crystal_sprites[Random.Range(0, crystal_sprites.Count)];
        }

        if(can_set_order)
            generateCrystalOrder();
    }

    public void generateCrystalOrder()
    {
        crystal_order = new List<int>();

        for (int i = 0; i < iterations; i++)
        {
            crystal_order.Add(Random.Range(0, crystals.Count));

            if(i > 0 && crystal_order[i-1] == crystal_order[i])
            {
                crystal_order[i] = Random.Range(0, crystals.Count);
            }
        }
    }

    public void checkCrystal(Crystal crystal)
    {
        int index = crystal_order[cur_trigger];

        Debug.Log(index);

        tries++;

        if (crystals[index] != crystal)
        {
            Debug.Log("wrong");
            cur_trigger = 0;
            source.clip = crystal_wrong;
            guessed_wrong = true;
            errors++;
            StartCoroutine(wrongCrystal());
        }
        else
        {
            Debug.Log("right");
            crystals[index].setPlayed(false);
            crystals[index].anim.changeAnimation("crystal_idle");
            crystals[index].anim.changeAnimation("crystal_blink");

            StartCoroutine(crystalGuess());

            cur_trigger++;
            source.clip = crystal_guess;

            if (cur_trigger == crystal_order.Count)
            {
                if (!guessed_wrong)
                {
                    firstTry++;
                }

                Debug.Log("guessed");
                trigger.doTrigger();
                guessed_order = true;
            }
        }
    }

    IEnumerator crystalGuess()
    {
        yield return new WaitForSeconds(.1f);

        source.Play();

    }

    IEnumerator crystalWrong()
    {
        for (int i = 0; i < 4; i++)
        {
            source.Play();

            yield return new WaitForSeconds(3 / 4);
        }
    }

    IEnumerator wrongCrystal()
    {
        StartCoroutine(crystalWrong());

        for (int i = 0; i < crystals.Count; i++)
        {
            crystals[i].interactable = false;
            crystals[i].crystal_light.color = Color.red;
            crystals[crystal_order[i]].anim.curAnimation = "";
            crystals[i].anim.changeAnimation("crystal_wrong");
        }

        yield return new WaitForSeconds(3f);

        for (int i = 0; i < crystals.Count; i++)
        {
            crystals[i].interactable = true;
            crystals[i].crystal_light.color = crystals[i].crystal_color;
        }

        yield return null;

        if (new_order_wrong)
        {
            if (can_set_order)
                generateCrystalOrder();
        }

        playCrystal();
    }

    public void playCrystal()
    {
        changeCrystalsInteract(false);

        StartCoroutine(playCrystalOrder());
    }

    IEnumerator playCrystalOrder()
    {
        source.clip = crystal_guess;
        for (int i = 0; i < crystal_order.Count; i++)
        {
            crystals[crystal_order[i]].setPlayed(false);
            crystals[crystal_order[i]].anim.changeAnimation("crystal_idle");
            crystals[crystal_order[i]].anim.changeAnimation("crystal_blink");

            source.Play();

            while(crystals[crystal_order[i]].played == false) { yield return null; }

            Debug.Log("Crystal stopped playing");
            yield return new WaitForSeconds(crystal_blink_delay);
        }

        changeCrystalsInteract(true);

        yield return null;
    }

    public void changeCrystalsInteract(bool interact)
    {
        for (int i = 0; i < crystals.Count; i++)
        {
            crystals[i].interactable = interact;
            crystals[i].played = true;
        }
    }
}
