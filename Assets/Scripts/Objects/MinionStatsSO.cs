using UnityEngine;

[CreateAssetMenu(fileName = "NewMinionStats", menuName = "Scriptable Objects/Minion")]
public class MinionStatsSO : ScriptableObject
{
    [Header("General Info")]
    public string minionName;
    public Sprite minionIcon;
    
    [Header("Prefabs")]
    [Tooltip("Il prefab usato in battaglia e nell'overworld")]
    public GameObject minionPrefab; 
    
    [Tooltip("Il prefab specifico per il menu di colorazione (con Drawing e Cardboard)")]
    public GameObject coloringPrefab; 

    [Header("Customization")]
    [Tooltip("La texture salvata dopo la colorazione")]
    public Texture2D savedCustomTexture;

    [Header("Base Stats")]
    public float maxHealth = 100f;
    public float attack = 10f;
    public float speed = 10f;
    public float defence = 5f;
    public float luck = 1f;

    [Header("Moves")]
    public MoveSO attackMove;
    public MoveSO defendMove;
    public MoveSO specialMove;
    public MoveSO fleeMove;

    [Header("Sounds")]
    public AudioClip[] damageSoundClips;
    public AudioClip specialSoundClip;
}
