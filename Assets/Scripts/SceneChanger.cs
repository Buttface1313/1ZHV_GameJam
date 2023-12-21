using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public void ChangeScene(string nextScene)
    {
        SceneManager.LoadScene(nextScene);
    }
    public void ChangeSceneAsync(string nextScene)
    {
        StartCoroutine(LoadNextLevel(nextScene));
    }

    private IEnumerator LoadNextLevel(string sceneName)
    {
        yield return SceneManager.LoadSceneAsync(sceneName);
    }
}
