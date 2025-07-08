using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class cutscene_object
{
    public string cutscene_name = "cutscene";
    public bool skippable = true;

    public float cutscene_start_delay = 0;

    public event_trigger[] cutscene_events;
    public event_trigger[] cutscene_end_events;

    public bool play_cutscene_after = false;
    public int nex_cutscene;
}

public class CutsceneManager : MonoBehaviour
{
    public int cutscene_to_play = 0;

    public List<cutscene_object> cutscenes;

    public static bool playingCutscene = false, playerMove = false, skipped = false;

    public Player player;

    public bool playOnStart = false;

    public CutsceneObject cutscene_object;
    public DialogManager dialog_manager;

    public GameObject cutsceneCanvas;

    private void Start()
    {
        skipped = false;

        if (playOnStart)
        {
            StartCoroutine(playCutscene());
        }
    }

    private void Update()
    {
        if (playingCutscene)
        {
            if (cutscenes[cutscene_to_play].skippable && Input.GetKeyDown(KeyCode.Z))
            {
                Debug.Log("aaa");

                skipped = true;

                StopAllCoroutines();

                StartCoroutine(playCutsceneSkip());
            }
        }
    }

    public void addPlayerMoveSpot(Transform spot)
    {
        player.walk_to = null;

        player.addWalkTo(spot);
    }

    public void changePlayerMove(bool move)
    {
        playerMove = move;
    }

    IEnumerator playCutscene()
    {
        playingCutscene = true;
        skipped = false;
        cutsceneCanvas.SetActive(true);

        yield return new WaitForSeconds(cutscenes[cutscene_to_play].cutscene_start_delay);

        for (int i = 0; i < cutscenes[cutscene_to_play].cutscene_events.Length; i++)
        {
            float before = cutscenes[cutscene_to_play].cutscene_events[i].thisTriggerDelay;
            float after = cutscenes[cutscene_to_play].cutscene_events[i].nextTriggerDelay;

            yield return new WaitForSeconds(before);

            cutscenes[cutscene_to_play].cutscene_events[i].eventsToTrigger.Invoke();

            if(cutscene_object != null)
            {
                while(cutscene_object.move_to.Count > 0)
                {
                    yield return null;
                }

                cutscene_object = null;
            }

            if(dialog_manager != null)
            {
                while (dialog_manager.writing) { yield return null; }

                dialog_manager = null;
            }

            yield return new WaitForSeconds(after);
        }

        for (int i = 0; i < cutscenes[cutscene_to_play].cutscene_end_events.Length; i++)
        {
            float before = cutscenes[cutscene_to_play].cutscene_end_events[i].thisTriggerDelay;
            float after = cutscenes[cutscene_to_play].cutscene_end_events[i].nextTriggerDelay;

            yield return new WaitForSeconds(before);

            cutscenes[cutscene_to_play].cutscene_end_events[i].eventsToTrigger.Invoke();

            yield return new WaitForSeconds(after);
        }

        Debug.Log("cutscene ended");
        playingCutscene = false;
        cutsceneCanvas.SetActive(false);
    }
    
    IEnumerator playCutsceneSkip()
    {
        playingCutscene = true;
        cutsceneCanvas.SetActive(true);

        yield return null;

        Debug.Log("cutscene ended");
        playingCutscene = false;
        skipped = false;
        cutsceneCanvas.SetActive(false);
    }

    public void playCutscene(int cutscene)
    {
        cutscene_to_play = cutscene;

        StartCoroutine(playCutscene());
    }

    public void waiUntilIsMoved(CutsceneObject co)
    {
        cutscene_object = co;
    }

    public void waiUntilIsWritten(DialogManager dm)
    {
        dialog_manager = dm;
    }

    public void setSkipped(bool skip)
    {
        skipped = skip;
    }
}
