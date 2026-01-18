using UnityEngine;

[CreateAssetMenu(fileName = "Move", menuName = "Scriptable Objects/Move")]
public class MoveSO : ScriptableObject
{
    [Header("Move Info")]
    public string moveName = "Basic Attack";

    public MoveType moveType;
    
    [Header("Move Stats")]
    public float attack = 0;
    public float defence = 0;
    public float luck = 0;
    public float speed = 0;
}
