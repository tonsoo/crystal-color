using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

[Serializable]
public class iteract_object_data
{
    public iteract_object_data(string n, int t)
    {
        object_name = n;
        times_interacted = t;
    }

    public string object_name;
    public int times_interacted;

    public int object_amount = 0;
}

public class GameManager : MonoBehaviour
{
    public UnityEvent events_to_start;

    public static string player_name = "";

    public string name = "tonso";

    bool escape_pressed = false;
    float ext_time = 0;
    public float exit_time = 2;

    public TextMeshProUGUI exit_text;

    public iteract_object_data[] estatisticas;

    void Start()
    {
        events_to_start.Invoke();

        player_name = name;

        iteract_object_data[] data = SaveManager.Load();

        if(data == null) { return; }

        for (int i = 0; i < data.Length; i++)
        {
            Debug.Log(data[i].object_name + " | " + data[i].times_interacted);
        }
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.F11))
        {
            Screen.fullScreen = !Screen.fullScreen;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            escape_pressed = true;
        }

        if (Input.GetKeyUp(KeyCode.Escape))
        {
            escape_pressed = false;
            ext_time = 0;
        }

        Color c = exit_text.color;

        c.a = (ext_time / exit_time) * 2;

        exit_text.color = c;

        if (escape_pressed)
        {
            ext_time += Time.deltaTime;

            if(ext_time > exit_time)
            {
                Debug.Log("Quiting game");
                Application.Quit();
            }
        }
    }

    public void loadData()
    {
        collectData();


        estatisticas = SaveManager.Load();
    }

    public void quitGame()
    {
        Application.Quit();
    }

    public void loadScene(string sceneName)
    {
        collectData();
        SceneManager.LoadScene(sceneName);
    }
    public void loadScene(int sceneIndex)
    {
        collectData();
        SceneManager.LoadScene(sceneIndex);
    }

    public void collectData()
    {
        List<iteract_object_data> objects = new List<iteract_object_data>();

        iteract_object_data[] dt_temp = SaveManager.Load();

        if(dt_temp != null)
        {
            for (int i = 0; i < dt_temp.Length; i++)
            {
                objects.Add(dt_temp[i]);
            }
        }

        DialogTrigger[] dts = FindObjectsOfType<DialogTrigger>();

        for (int i = 0; i < dts.Length; i++)
        {
            iteract_object_data iod = new iteract_object_data(dts[i].object_name, dts[i].times_interacted);

            bool add = true;
            int a = -1;

            for (int j = 0; j < objects.Count; j++)
            {
                if (objects[j].object_name == dts[i].object_name)
                {
                    objects[j].object_amount++;
                    add = false;
                    a = j;
                    break;
                }
            }

            if (add && dts[i].object_name != "")
            {
                objects.Add(iod);
            }
            else
            {
                if(a >= 0)
                {
                    if (dts[i].count_once && dts[i].times_interacted != 0)
                    {
                        objects[a].times_interacted += 1;
                    }
                    else if (!dts[i].count_once)
                    {
                        objects[a].times_interacted += dts[i].times_interacted;
                    }
                }
            }
        }

        iteract_object_data[] dt = new iteract_object_data[objects.Count];

        for (int i = 0; i < objects.Count; i++)
        {
            dt[i] = objects[i];
            Debug.Log(objects[i].object_name + ": " + objects[i].times_interacted);
        }

        SaveManager.Save(dt);
    }

    public static string replace(string s, string rpc, string value_replace)
    {
        if(!s.Contains(rpc)) { return s; }

        string compare = s, final = "";

        while (compare.Contains(rpc))
        {
            string ns = "";
            int start = 0, end = 0;

            for (int i = 0, j = 0; i < compare.Length; i++)
            {
                if (compare[i] == rpc[j])
                {
                    if (j == 0) { start = i; }
                    if (j == rpc.Length - 1) { end = i + 1; }

                    ns += compare[i];
                    j++;
                }
                else
                {
                    start = 0; end = 0;
                    j = 0;
                    ns = "";
                }

                if (ns == rpc)
                {
                    string b = compare.Substring(0, start);
                    Debug.Log("end: " + end + " | comparelength: " + (compare.Length - 1));
                    string c = compare.Substring(end);
                    compare = b + value_replace + c;
                    break;
                }
            }
        }

        return compare;
    }
}
