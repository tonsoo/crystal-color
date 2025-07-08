using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static int level = 0;

    public static void loadLevel(int level)
    {
        LevelManager.level = level;
        SceneManager.LoadScene("level_selector");
    }
}
