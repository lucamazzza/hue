using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    [Header("Scene Settings")]
    [Tooltip("Name of the scene to load (must be in Build Settings)")]
    public string targetSceneName;
    
    [Header("Trigger Settings")]
    [Tooltip("How should this transition be triggered?")]
    public TriggerType triggerType = TriggerType.OnTriggerEnter;
    
    [Tooltip("Required tag for trigger (leave empty for any tag)")]
    public string requiredTag = "Player";
    
    [Tooltip("Key to press when using KeyPress trigger")]
    public KeyCode activationKey = KeyCode.E;
    
    [Tooltip("Show UI prompt when player is in range?")]
    public bool showPrompt = true;
    
    [Header("Spawn Point")]
    [Tooltip("Where should the player spawn in the target scene? (optional)")]
    public string spawnPointName = "SpawnPoint";
    
    private bool playerInRange = false;
    private GameObject promptUI;

    public enum TriggerType
    {
        OnTriggerEnter,      // Automatic when entering trigger
        OnCollisionEnter,    // Automatic when colliding
        KeyPress,            // Press key when in trigger zone
        ExternalCall         // Call LoadScene() from another script/UI
    }

    void Start()
    {
        if (showPrompt && triggerType == TriggerType.KeyPress)
        {
            CreatePromptUI();
        }
    }

    void Update()
    {
        if (triggerType == TriggerType.KeyPress && playerInRange && Input.GetKeyDown(activationKey))
        {
            LoadScene();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!IsValidObject(other.gameObject)) return;

        if (triggerType == TriggerType.OnTriggerEnter)
        {
            LoadScene();
        }
        else if (triggerType == TriggerType.KeyPress)
        {
            playerInRange = true;
            if (promptUI != null) promptUI.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (!IsValidObject(other.gameObject)) return;

        if (triggerType == TriggerType.KeyPress)
        {
            playerInRange = false;
            if (promptUI != null) promptUI.SetActive(false);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (triggerType == TriggerType.OnCollisionEnter && IsValidObject(collision.gameObject))
        {
            LoadScene();
        }
    }

    public void LoadScene()
    {
        if (string.IsNullOrEmpty(targetSceneName))
        {
            Debug.LogError("Target scene name is not set!");
            return;
        }

        PlayerPrefs.SetString("SpawnPointName", spawnPointName);
        PlayerPrefs.Save();
        
        SceneManager.LoadScene(targetSceneName);
    }

    bool IsValidObject(GameObject obj)
    {
        if (string.IsNullOrEmpty(requiredTag)) return true;
        return obj.CompareTag(requiredTag);
    }

    void CreatePromptUI()
    {
        promptUI = new GameObject("TransitionPrompt");
        promptUI.transform.SetParent(transform);
        promptUI.transform.localPosition = Vector3.up * 2f;
        
        var canvas = promptUI.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.GetComponent<RectTransform>().sizeDelta = new Vector2(200, 50);
        
        var textObj = new GameObject("Text");
        textObj.transform.SetParent(promptUI.transform);
        
        var text = textObj.AddComponent<UnityEngine.UI.Text>();
        text.text = $"Press {activationKey} to enter";
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = 14;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = Color.white;
        text.rectTransform.sizeDelta = new Vector2(200, 50);
        
        promptUI.SetActive(false);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, transform.localScale);
    }
}
