using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.Audio;

[System.Serializable]
public class event_trigger
{
    public float thisTriggerDelay = 0;
    public UnityEvent eventsToTrigger;
    public float nextTriggerDelay = 0;
}

[Serializable]
public class dialog_content
{
    public dialog_content() { }
    public dialog_content(string text, float speed, bool skippable, AudioClip clip, int audio_skip)
    {
        dialog_text = text;
        writing_speed = speed;
        this.skippable = skippable;
        character_audio = clip;
        character_audio_skip = audio_skip;
    }

    [TextArea(3, 7)]
    public string dialog_text = "";
    public float writing_speed = 1;

    public bool skippable = true;
    public bool skip_with_time = false;

    public float time_to_skip = 1;

    public event_trigger[] events_to_trigger;

    public bool after_skip_dialog = false;
    public int gotoDialog = 0;

    public int character_audio_skip = 2;
    public AudioClip character_audio;

    public int character_frame_skip = 2;
    public Sprite[] character;
}

public class DialogManager : MonoBehaviour
{
    public bool writing = false;
    bool skipped = false, canskip = false;

    public int current_dialog = 0;
    public List<dialog_content> content;

    public GameObject dialog_canvas;
    public TextMeshProUGUI dialog_text;
    public Image dialog_character;
    public AudioSource dialog_audio;
    public AudioMixerGroup pitchBendGroup;

    string dialog = "";
    float write_delay = 0;

    public Player player;

    public Vector2 pos_with_character;
    public Vector2 size_with_character;

    public Vector2 pos_without_character;
    public Vector2 size_without_character;

    void Start()
    {
        closeDialog();

        Debug.Log(dialog_text.rectTransform.localPosition + " | " + dialog_text.rectTransform.sizeDelta);
    }

    void Update()
    {
        if (writing)
        {
            if (content[current_dialog].skippable)
            {
                if (Input.GetKeyDown(KeyCode.Z) && canskip)
                {
                    if (!skipped)
                    {
                        skipped = true;
                        dialog_text.text = content[current_dialog].dialog_text.ToUpper();
                    }
                }
            }
        }
    }

    public void closeDialog()
    {
        StopAllCoroutines();

        dialog_canvas.SetActive(false);
        dialog_text.text = "";
        dialog_character.sprite = null;
        dialog_character.enabled = false;

        content = null;
        current_dialog = 0;
        writing = false;
    }

    public void setDialog(List<dialog_content> new_dialog)
    {
        if (writing) { return; }

        dialog_canvas.SetActive(true);
        dialog_text.text = "";

        current_dialog = 0;
        content = new_dialog;

        StartCoroutine(writeText());
    }

    public void setDialogPriority(List<dialog_content> new_dialog)
    {
        dialog_canvas.SetActive(true);
        dialog_text.text = "";

        current_dialog = 0;
        content = new_dialog;

        StopAllCoroutines();
        StartCoroutine(writeText());
    }

    IEnumerator writeText()
    {
        canskip = false;
        writing = true;
        if (player != null) { player.canWalk = false; }

        current_dialog = 0;

        while(current_dialog < content.Count)
        {
            skipped = false;

            string[] player_name = new string[]{
                "${player_name}",
                "${PLAYER_NAME}"
            };
            string d = GameManager.replace(content[current_dialog].dialog_text, "${player_name}" ,GameManager.player_name);
            dialog = d.ToUpper();
            write_delay = content[current_dialog].writing_speed;

            dialog_text.text = "";

            if (content[current_dialog].character.Length == 0)
            {
                dialog_character.enabled = false;
                dialog_character.sprite = null;
                dialog_text.rectTransform.sizeDelta = size_without_character;//new Vector2(687.5387f, 130.4394f)
                dialog_text.transform.localPosition = pos_without_character;//Vector2.zero
            }
            else
            {
                dialog_character.enabled = true;
                dialog_character.sprite = content[current_dialog].character[0];
                dialog_text.rectTransform.sizeDelta = size_with_character;//new Vector2(550.5616f, 130.4394f)
                dialog_text.transform.localPosition = pos_with_character;//new Vector2(65.45f, 0)
            }

            dialog_audio.clip = null;

            if (content[current_dialog].character_audio != null)
            {
                dialog_audio.clip = content[current_dialog].character_audio;
            }

            for (int i = 0, sk = 0, ch = 1, audio_skip = 0; i < dialog.Length; i++, sk++, audio_skip++)
            {
                if (skipped || (CutsceneManager.skipped)) { break; }

                if(dialog_audio.clip != null && audio_skip >= content[current_dialog].character_audio_skip) { dialog_audio.Play(); audio_skip = 0; }

                yield return new WaitForSeconds(write_delay);

                if (!skipped || (!CutsceneManager.skipped))
                {
                    dialog_text.text += dialog[i];

                    if (content[current_dialog].character.Length > 0 && sk >= content[current_dialog].character_frame_skip)
                    {
                        if (ch >= content[current_dialog].character.Length) { ch = 0; }

                        dialog_character.sprite = content[current_dialog].character[ch];
                        sk = 0;
                        ch++;
                    }
                }

                canskip = true;
            }

            dialog_text.text = dialog;

            skipped = false;

            if (content[current_dialog].character.Length > 0) { dialog_character.sprite = content[current_dialog].character[0]; }

            if (content[current_dialog].skip_with_time)
            {
                if(!CutsceneManager.skipped)
                    yield return new WaitForSeconds(content[current_dialog].time_to_skip);
            }
            else
            {
                while (!skipped) { yield return null;  }
            }

            if (content[current_dialog].events_to_trigger.Length > 0)
            {
                for (int i = 0; i < content[current_dialog].events_to_trigger.Length; i++)
                {
                    if (!CutsceneManager.skipped)
                        yield return new WaitForSeconds(content[current_dialog].events_to_trigger[i].thisTriggerDelay);

                    content[current_dialog].events_to_trigger[i].eventsToTrigger.Invoke();

                    if (!CutsceneManager.skipped)
                        yield return new WaitForSeconds(content[current_dialog].events_to_trigger[i].nextTriggerDelay);
                }
            }

            if (content[current_dialog].after_skip_dialog)
            {
                current_dialog = content[current_dialog].gotoDialog;
            }
            else
            {
                current_dialog++;
            }
        }

        writing = false;
        if (player != null) { player.canWalk = true; }
        closeDialog();
        canskip = false;
    }

    public void setPosition(Transform newPosition)
    {
        dialog_canvas.transform.position = newPosition.position;
    }

    public void setBackground(Sprite newBg)
    {
        dialog_canvas.GetComponent<Image>().sprite = newBg;
    }

    public void setSizeWithCharacter(Vector2 newSize)
    {
        size_with_character = newSize;
    }
    public void setSizeWithoutCharacter(Vector2 newSize)
    {
        size_without_character = newSize;
    }
    public void setSize(Vector2 newSize)
    {
        size_without_character = newSize;
        size_with_character = newSize;
    }

    public void setPositionWithCharacter(Vector2 newPosition)
    {
        pos_with_character = newPosition;
    }
    public void setPositionWithoutCharacter(Vector2 newPosition)
    {
        pos_without_character = newPosition;
    }
    public void setPosition(Vector2 newPosition)
    {
        pos_without_character = newPosition;
        pos_with_character = newPosition;
    }

    public void setSizeWithCharacter(RectTransform newSize)
    {
        size_with_character = newSize.sizeDelta;
    }
    public void setSizeWithoutCharacter(RectTransform newSize)
    {
        size_without_character = newSize.sizeDelta;
    }
    public void setSize(RectTransform newSize)
    {
        size_without_character = newSize.sizeDelta;
        size_with_character = newSize.sizeDelta;
    }

    public void setPositionWithCharacter(RectTransform newPosition)
    {
        pos_with_character = newPosition.transform.position;
    }
    public void setPositionWithoutCharacter(RectTransform newPosition)
    {
        pos_without_character = newPosition.transform.position;
    }
    public void setPosition(RectTransform newPosition)
    {
        pos_without_character = newPosition.transform.position;
        pos_with_character = newPosition.transform.position;
    }
}
