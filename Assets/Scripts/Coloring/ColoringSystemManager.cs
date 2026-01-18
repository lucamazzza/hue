using UnityEngine;

public class ColoringSystemManager : MonoBehaviour
{
    public static ColoringSystemManager Instance { get; private set; }

    [Header("Riferimenti UI e Logica")]
    public GameObject coloringUIPanel;
    public ColoringController coloringController;
    public Transform spawnPoint;
    public Camera coloringCamera; // La camera dedicata

    private GameObject currentMonsterInstance;
    private MinionStatsSO currentStats;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (coloringUIPanel != null) coloringUIPanel.SetActive(false);
        if (coloringCamera != null) coloringCamera.gameObject.SetActive(false);
    }

    public void OpenColoringMenu(MinionStatsSO stats)
    {
        if (stats == null || stats.coloringPrefab == null) return;

        currentStats = stats;
        coloringUIPanel.SetActive(true);

        if (coloringCamera != null) coloringCamera.gameObject.SetActive(true);

        if (currentMonsterInstance != null) Destroy(currentMonsterInstance);

        currentMonsterInstance = Instantiate(stats.coloringPrefab, spawnPoint.position, spawnPoint.rotation);

        if (coloringController != null)
        {
            coloringController.Initialize(currentMonsterInstance, stats.savedCustomTexture);
        }
    }

    public void CloseColoringMenu()
    {
        if (currentStats != null && coloringController != null)
        {
            currentStats.savedCustomTexture = coloringController.GetCurrentTexture();
        }

        if (coloringUIPanel != null) coloringUIPanel.SetActive(false);
        if (coloringCamera != null) coloringCamera.gameObject.SetActive(false);
        if (currentMonsterInstance != null) Destroy(currentMonsterInstance);
        
        currentStats = null;
    }
}