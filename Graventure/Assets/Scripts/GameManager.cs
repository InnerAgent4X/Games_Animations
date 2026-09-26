using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; set; }

    public float bpm = 60f;

    public int subdivision = 1;
    public event Action<int> OnBeat; // Event that is triggered on each beat

    double dspNextBeatTime;
    double dspStartTime;
    double beatInterval => levelSettings.EnemySpeed / bpm / subdivision;
    long beatCount = 0;



    public int Score = 0;
    public int HealthyBoy;
    public bool isGameOver = false;
    public bool isYouWin = false;
    public bool playerOnCooldown = false;
    public Level_Settings levelSettings;
    public Player player;
    public GameObject resultsPanel;
    public TextMeshProUGUI resultsText;
    

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
        resultsPanel.SetActive(false);
    }

    void Update()
    {
        if (HealthyBoy <= 0)
        {
            isGameOver = true;
            resultsText.text = "You Died...";
            resultsPanel.SetActive(true);
        }
        
        if (Score >= levelSettings.VictoryScore)
        {
            isYouWin = true;
            resultsText.text = "You Win!";
            resultsPanel.SetActive(true);
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

    public void AttackPenalty()
    {
        player.cooldown();
    }

    public void ReturnToMenu()
    {
        PlayerPrefs.SetInt("menuAdvance", 1);
        SceneManager.LoadScene(0);
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.SetInt("menuAdvance", 0);
    }

}
