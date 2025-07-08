using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class dialog_trigger_dialogs
{
    public bool go_to_next_when_interacted = false;
    public int times_to_next = 1;
    public List<dialog_content> dialogs;

    public bool destroy_script_after_this = false;
    public bool destroy_object_after_this = false;
}
public class DialogTrigger : MonoBehaviour
{
    public int dialog_to_send = 0;

    public dialog_trigger_dialogs[] dialogs;

    public DialogManager dialog_manager;

    public bool interact_to_start = false, interactable = true;
    public bool destroy_script_after_use = false;
    public bool destroy_object_after_use = false;
    public Player player;
    public LayerMask playerLayer;

    public string object_name = "";
    public bool count_once = false;
    public int times_interacted = 0;
    int total_times = 0;


    void Start()
    {
        times_interacted = 0;
        player = FindObjectOfType<Player>();
    }

    void Update()
    {
        if(dialogs.Length > 0)
        {
            if (dialogs[dialog_to_send].go_to_next_when_interacted)
            {
                if(times_interacted >= dialogs[dialog_to_send].times_to_next)
                {
                    dialog_to_send++;
                    times_interacted = 0;

                    if(dialog_to_send >= dialogs.Length)
                    {
                        Destroy(this);
                    }
                }
            }
        }

        if (interact_to_start && player != null)
        {
            if (Input.GetKeyDown(KeyCode.Z) && interactable && player.interactable == gameObject)
            {
                startDialog();
            }
        }
    }

    public void startDialog()
    {
        if (dialog_manager.writing) { return; }

        times_interacted++;
        total_times++;
        dialog_manager.setDialog(dialogs[dialog_to_send].dialogs);

        if (dialogs[dialog_to_send].destroy_script_after_this || destroy_script_after_use) { Destroy(this); }
        if (dialogs[dialog_to_send].destroy_object_after_this || destroy_object_after_use) { Destroy(gameObject); }
    }
    public void startDialogWithDelay(float delay)
    {
        StartCoroutine(startDialog(delay));
    }

    public void startDialogPriotiry()
    {
        times_interacted++;
        total_times++;
        dialog_manager.setDialogPriority(dialogs[dialog_to_send].dialogs);

        if (dialogs[dialog_to_send].destroy_script_after_this || destroy_script_after_use) { Destroy(this); }
        if (dialogs[dialog_to_send].destroy_object_after_this || destroy_object_after_use) { Destroy(gameObject); }
    }

    IEnumerator startDialog(float delay)
    {
        yield return new WaitForSeconds(delay);

        startDialog();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("player"))
        {
            if (!interact_to_start)
            {
                if (collision.gameObject.GetComponent<Player>().walk_to.Count == 0)
                {
                    startDialog();

                    if (destroy_script_after_use) { Destroy(gameObject); }
                }
            }
        }
    }
}
