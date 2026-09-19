using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; set; }

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
    }
}
