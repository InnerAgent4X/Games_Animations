using UnityEngine;

[CreateAssetMenu(fileName = "Level_Settings", menuName = "Scriptable Objects/Level_Settings")]
public class Level_Settings : ScriptableObject
{
    public int timer;
    public int enemyCount;
    public int playerHealth;
    public int enemySpeed;

}
