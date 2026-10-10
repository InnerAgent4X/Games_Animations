using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; set; }

    public float bpm;

    public int subdivision = 1;
    public event Action<int> OnBeat; // Event that is triggered on each beat

    double dspNextBeatTime;
    double dspStartTime;
    double beatInterval => 60 / bpm / subdivision;
    long beatCount = 0;



    public int Score = 0;
    public int HealthyBoy;
    public bool isGameOver = false;
    public bool isYouWin = false;
    public bool playerOnCooldown = false;
    public Level_Settings levelSettings;
    public Player player;
    public GameObject resultsPanel;
    public GameObject controlPanel;
    public TextMeshProUGUI resultsText;
    public TextMeshProUGUI resultsScore;
    public TextMeshProUGUI scoreText;

    private bool endlessMode = false;


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
        bpm = levelSettings.EnemySpeed;
        // Initialize DSP timing so beats start in sync with AudioSettings.dspTime
        // small startup offset to avoid firing many beats immediately
        dspStartTime = AudioSettings.dspTime + 0.1f;
        dspNextBeatTime = dspStartTime;
        resultsPanel.SetActive(false);
        if(levelSettings.VictoryScore <= 0)
        {
            endlessMode = true;
        }
    }

    void Update()
    {
        if (HealthyBoy <= 0)
        {
            isGameOver = true;
            resultsText.text = "You Died...";
            resultsScore.text = "Score: " + Score;
            controlPanel.SetActive(false);
            resultsPanel.SetActive(true);
        }
        
        if (!endlessMode && Score >= levelSettings.VictoryScore)
        {
            isYouWin = true;
            resultsText.text = "You Win!";
            resultsScore.text = "Score: " + Score;
            controlPanel.SetActive(false);
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

    public void scoreCount()
    {
        scoreText.text = "Score: " + Score;
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
