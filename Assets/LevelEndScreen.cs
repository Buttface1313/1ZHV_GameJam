using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelEndScreen : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _timeText;
    [SerializeField] string nextScene;
    [SerializeField] PauseScreen _pauseScreen;
    private void OnEnable()
    {
        Time.timeScale = 0;
        GameManager.Instance.GetCurrentTime(out float startTime, out float totalTime);
        _timeText.text = $"Level time:\n{getFormatedTime(Time.time - startTime)}\nTotal time:\n{getFormatedTime(Time.time - totalTime)}";
        _pauseScreen.Menu.Disable();
        _pauseScreen.MouseMovement.Disable();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void LoadNextScene()
    {
        SceneManager.LoadScene(nextScene);
    }
    public void ReturnToMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    string getFormatedTime(float currentTime)
    {
        int minutes = (int)currentTime / 60;
        string seconds = ((int)currentTime % 60).ToString("D2");
        string miliseconds = ((int)(currentTime * 100) % 100).ToString("D2");
        return $"{minutes}:{seconds}:{miliseconds}";
    }
}
