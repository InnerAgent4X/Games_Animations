using UnityEngine;

[CreateAssetMenu(fileName = "Level_Settings", menuName = "Scriptable Objects/Level_Settings")]
public class Level_Settings : ScriptableObject
{
    public int VictoryScore;
    public int PlayerHealth;
    public float EnemySpawnRate;
    public int EnemySpeed;
    public int PenaltyDuration;
}
