using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractTrigger : MonoBehaviour
{
    public List<event_trigger> events_to_trigger;

    public bool interact_to_trigger = false, destroy_script_sfter_trigger = false,
        destroy_object_after_trigger, interactable;
    bool interacted = false;

    public Player player;

    float distance = .5f;
    Transform position_to_wait, position_target;

    AudioSource source;
    public float audio_delay;
    public AudioClip object_clip;

    private void Start()
    {
        player = FindObjectOfType<Player>();

        if (gameObject.GetComponent<AudioSource>() && source == null)
        {
            source = gameObject.GetComponent<AudioSource>();
        }

        if(source != null)
        {
            source.clip = object_clip;
        }
    }

    void Update()
    {
        if(player != null)
        {
            if (Input.GetKeyDown(KeyCode.Z) && interact_to_trigger && interactable && player.interactable == gameObject)
            {
                doTrigger();
            }
        }
    }

    public void doTrigger()
    {
        if(interacted && (destroy_object_after_trigger || destroy_script_sfter_trigger)) { return; }

        playObjectAudio();

        StartCoroutine(triggerEvents());
        interacted = true;
    }

    public void playObjectAudio()
    {
        StartCoroutine(playAudio());
    }

    IEnumerator playAudio()
    {
        yield return new WaitForSeconds(audio_delay);

        if (source != null) { source.Play(); }
    }

    IEnumerator triggerEvents()
    {
        for (int i = 0; i < events_to_trigger.Count; i++)
        {
            float before = events_to_trigger[i].thisTriggerDelay;
            float after = events_to_trigger[i].nextTriggerDelay;

            yield return new WaitForSeconds(before);

            events_to_trigger[i].eventsToTrigger.Invoke();

            if(position_target != null && position_to_wait != null)
            {
                while (Vector2.Distance(position_target.position, position_to_wait.position) > distance) { yield return null; }

                position_target = null;
                position_to_wait = null;
            }

            yield return new WaitForSeconds(after);
        }

        if (destroy_script_sfter_trigger) { Destroy(this); }
        if (destroy_object_after_trigger) { Destroy(gameObject); }
    }

    public void setDistance(float d) { distance = d; }

    public void setPositionTarget(Transform position)
    {
        position_target = position;
    }

    public void waitUntilPosition(Transform position)
    {
        position_to_wait = position;
    }

    public void changeInteractable(bool i)
    {
        interactable = i;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("player"))
        {
            if (!interact_to_trigger)
            {
                doTrigger();
            }
        }
    }
}
