using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.Events;
using Unity.VisualScripting;
using JetBrains.Annotations;
using UnityEngine.SceneManagement;

[Serializable]
public class menu_option
{
    public Image option_image = null;
    public TextMeshProUGUI option_text = null;
    public string option_default_text = "";

    public event_trigger[] events_to_trigger;
}

[Serializable]
public class menu_option_layer
{
    public menu_option[] options;
}

public class MenuController : MonoBehaviour
{
    public Color default_color, selected_color;

    public int current_layer = 0, current_option = 0;
    public menu_option_layer[] options;

    bool canInteract = true;
    public bool wait_to_interact = false;
    public float time_to_wait = 1;
    float t = 0;

    public UnityEvent events_to_start;

    public DialogManager dialogManager;

    private void Start()
    {
        for (int j = 0; j < options.Length; j++)
        {
            for (int i = 0; i < options[j].options.Length; i++)
            {
                if (options[j].options[i].option_text != null)
                {
                    options[j].options[i].option_default_text = options[j].options[i].option_text.text;
                }
            }
        }

        if (options[current_layer].options[current_option].option_image != null) { options[current_layer].options[current_option].option_image.color = selected_color; }
        if (options[current_layer].options[current_option].option_text != null)
        {
            options[current_layer].options[current_option].option_text.color = selected_color;
            options[current_layer].options[current_option].option_text.text = "> " + options[current_layer].options[current_option].option_default_text;
        }

        events_to_start.Invoke();
    }

    private void Update()
    {
        updateOptions();

        if (wait_to_interact)
        {
            t += Time.deltaTime;

            if(t >= time_to_wait)
            {
                t = 0;
                wait_to_interact = false;
                canInteract = true;
            }
            return;
        }

        if (dialogManager.writing) { return; }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            current_option--;

            if(current_option < 0) { current_option = options[current_layer].options.Length - 1; }
        }
        else if(Input.GetKeyDown(KeyCode.DownArrow))
        {
            current_option++;

            if(current_option >= options[current_layer].options.Length) { current_option = 0; }
        }

        if (Input.GetKeyDown(KeyCode.Z) && canInteract)
        {
            Debug.Log("aaa");
            if (options[current_layer].options[current_option].events_to_trigger.Length > 0)
            {
                StartCoroutine(invokeEvents());
            }
        }
    }

    public void updateOptions()
    {
        for (int i = 0; i < options[current_layer].options.Length; i++)
        {
            if (i == current_option)
            {
                if (options[current_layer].options[i].option_image != null) { options[current_layer].options[i].option_image.color = selected_color; }
                if (options[current_layer].options[i].option_text != null)
                {
                    options[current_layer].options[i].option_text.color = selected_color;
                    options[current_layer].options[i].option_text.text = "> " + options[current_layer].options[i].option_default_text;
                }

                continue;
            }

            if (options[current_layer].options[i].option_image != null) { options[current_layer].options[i].option_image.color = default_color; }
            if (options[current_layer].options[i].option_text != null)
            {
                options[current_layer].options[i].option_text.color = default_color;
                options[current_layer].options[i].option_text.text = options[current_layer].options[i].option_default_text;
            }
        }
    }

    public void setWaitTime(float wait_time)
    {
        wait_to_interact = true;
        time_to_wait = wait_time;
        t = 0;
        canInteract = false;
    }

    public void changeOptionLayer(int newLayer)
    {
        if (newLayer >= options.Length)
            return;

        StartCoroutine(changeLayer(newLayer));
    }

    IEnumerator changeLayer(int layer)
    {
        yield return new WaitForSeconds(.1f);

        current_layer = layer;
        current_option = 0;
    }

    IEnumerator invokeEvents()
    {
        canInteract = false;

        for (int i = 0; i < options[current_layer].options[current_option].events_to_trigger.Length; i++)
        {
            float now = options[current_layer].options[current_option].events_to_trigger[i].thisTriggerDelay;
            float after = options[current_layer].options[current_option].events_to_trigger[i].nextTriggerDelay;

            yield return new WaitForSeconds(now);

            options[current_layer].options[current_option].events_to_trigger[i].eventsToTrigger.Invoke();

            yield return new WaitForSeconds(after);
        }

        yield return new WaitForSeconds(.1f);

        canInteract = true;
    }

    public void loadScene(int scene_index)
    {
        SceneManager.LoadScene(scene_index);
    }

    public void loadScene(string scene_name)
    {
        SceneManager.LoadScene(scene_name);
    }
}
