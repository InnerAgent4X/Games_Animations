using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; set; }

    public float bpm = 60f;

    public int subdivision = 1;
    public event Action<int> OnBeat; // Event that is triggered on each beat

    double dspNextBeatTime;
    double dspStartTime;
    double beatInterval => 60.0 / bpm / subdivision;
    long beatCount = 0;



    public int Score = 0;
    public int HealthyBoy;
    public bool isGameOver = false;
    public bool isYouWin = false;
    public Level_Settings levelSettings;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        HealthyBoy = levelSettings.PlayerHealth;

        // Initialize DSP timing so beats start in sync with AudioSettings.dspTime
        // small startup offset to avoid firing many beats immediately
        dspStartTime = AudioSettings.dspTime + 0.1f;
        dspNextBeatTime = dspStartTime;
    }

    void Update()
    {
        if (HealthyBoy <= 0)
        {
            isGameOver = true;
        }
        
        if (Score >= levelSettings.VictoryScore)
        {
            isYouWin = true;
        }

        var dspTime = AudioSettings.dspTime;
        while (dspTime >= dspNextBeatTime)
        {
            FireBeat((int)beatCount);
            beatCount++;
            dspNextBeatTime += beatInterval;
        }

    }

    void FireBeat(int beatNumber)
    {
        OnBeat?.Invoke(beatNumber);
    }

    public float GetBeatProgress()
    {
        var dspTime = (float)AudioSettings.dspTime;
        var prevBeat = (float)(dspNextBeatTime - beatInterval);
        if (beatInterval <= 0) return 0f;
        return Mathf.Clamp01((dspTime - prevBeat) / (float)beatInterval);
    }

    public float GetBeatIntervalSeconds() => (float)beatInterval;

}
