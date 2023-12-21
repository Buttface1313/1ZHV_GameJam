using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public Transform CurrentRespawnPoint;

    private int _partOfLevel;
    [SerializeField] private List<int> _numOfLights;
    [SerializeField] private float lightsTurningOnSpeed =1.0f;

    public UnityEvent<int, int> NotifyLights;

    public UnityEvent PlayerRespawned;

    public float levelStartTime = 0;

    private float _startTime;
    private static float _totalTime;
    private static Dictionary<string, float> _timesPerScene = new();

    private void Awake()
    {
        Time.timeScale = 1.0f;
        if (Instance == null)
            Instance = this;
        else
            Debug.LogError("TOO MANY GAME MANAGERS");
        PlayerRespawned?.Invoke();
        if (SceneManager.GetActiveScene().name == "First Level")
        {
            _timesPerScene.Clear();
            _totalTime = 0;
        }
    }
    private void OnDestroy()
    {
        Instance = null;

    }

    public void StartTimer()
    {
        _startTime = Time.time;
        if (SceneManager.GetActiveScene().name == "First Level")
            _totalTime = Time.time;
    }

    public void SaveTime()
    {
        if(_timesPerScene.TryAdd(SceneManager.GetActiveScene().name,Time.time - _startTime))
        {
            _timesPerScene[SceneManager.GetActiveScene().name]=(Time.time - _startTime);
        }
    }

    public void GetCurrentTime(out float startTime, out float totalStartTime)
    {
        startTime = _startTime;
        totalStartTime = _totalTime;
    }
}
