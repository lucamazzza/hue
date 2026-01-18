[System.Serializable]
public class MinionData 
{
    public MinionStatsSO stats;
    public float currentHealth;
    
    public MinionData(MinionStatsSO baseStats) {
        stats = baseStats;
        currentHealth = baseStats.maxHealth;
    }
}