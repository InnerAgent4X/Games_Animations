using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuManager : MonoBehaviour
{
    public GameObject levelPanel;
    public GameObject levelInfoPanel;

    public Level_Settings[] levelSettings;

    public TextMeshProUGUI levelNameText;
    public TextMeshProUGUI requiredScoreText;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI enemySpeedText;

    private int selectedLevelIndex = -1;

    private void Start()
    {
        int menuAdvance = PlayerPrefs.GetInt("menuAdvance", 0);
        if (menuAdvance == 0) levelPanel.SetActive(false);
        else if (menuAdvance == 1) levelPanel.SetActive(true);
        levelInfoPanel.SetActive(false);
    }

    public void StartButton()
    {
        levelPanel.SetActive(true);
    }

    public void LevelSelected(int levelIndex)
    {
        levelNameText.text = "Level " + (levelIndex);
        selectedLevelIndex = levelIndex;
        requiredScoreText.text = "Required Score: " + levelSettings[levelIndex -1].VictoryScore;
        healthText.text = "Health: " + levelSettings[levelIndex -1].PlayerHealth;
        enemySpeedText.text = "Enemy Speed: " + levelSettings[levelIndex -1].EnemySpeed;

        levelInfoPanel.SetActive(true);
    }

    public void CloseLevelInfo()
    {
        selectedLevelIndex = -1;
        levelInfoPanel.SetActive(false);
    }

    public void LoadLevel()
    {

        if (selectedLevelIndex == -1 || selectedLevelIndex >= SceneManager.sceneCountInBuildSettings) return;

        SceneManager.LoadScene(selectedLevelIndex);
    }
    private void OnApplicationQuit()
    {
        PlayerPrefs.SetInt("menuAdvance", 0);
    }
}
