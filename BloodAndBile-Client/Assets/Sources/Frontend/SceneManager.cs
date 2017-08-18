using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneManagement
{
    static public string GetCurrentSceneName()
    {
        return SceneManager.GetActiveScene().name;
    }

    static public void SwitchToScene(string sceneName)
    {
        SceneManager.LoadSceneAsync(sceneName);
    }
}