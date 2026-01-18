using UnityEngine;

[CreateAssetMenu(fileName = "EnemyDifficulty", menuName = "Scriptable Objects/EnemyDifficulty")]
public class EnemyDifficultySO : ScriptableObject
{
    [Header("Defense")]
    [Range(0.0f, 1.0f)]
    public float healthThresholdDefense = 0.0f;

    [Header("Behaviour")]
    [Range(0.0f, 1.0f)]
    public float randomActionChance = 0.0f;

    public int attackWeight = 0;
    public int defenceWeight = 0;
    public int specialWeight = 0;

    [Header("Intelligence")]
    [Range(0.0f, 1.0f)]
    public float exploitWeaknessChance = 0.0f;
}