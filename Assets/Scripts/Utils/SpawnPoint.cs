using UnityEngine;
using UnityEngine.SceneManagement;

public class SpawnPoint : MonoBehaviour
{
    [Header("Spawn Point Settings")]
    [Tooltip("Unique name for this spawn point")]
    public string spawnPointName = "SpawnPoint";
    
    [Tooltip("Automatically spawn player at this point on scene load?")]
    public bool isDefaultSpawn = true;

    void Start()
    {
        string requestedSpawn = PlayerPrefs.GetString("SpawnPointName", "");
        
        if ((isDefaultSpawn && string.IsNullOrEmpty(requestedSpawn)) || 
            requestedSpawn == spawnPointName)
        {
            SpawnPlayer();
            PlayerPrefs.DeleteKey("SpawnPointName");
        }
    }

    void SpawnPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        
        if (player != null)
        {
            CharacterController characterController = player.GetComponent<CharacterController>();
            if (characterController != null)
            {
                characterController.enabled = false;
                player.transform.position = transform.position;
                player.transform.rotation = transform.rotation;
                characterController.enabled = true;
            }
            else
            {
                player.transform.position = transform.position;
                player.transform.rotation = transform.rotation;
            }
            
            Debug.Log($"Player spawned at {spawnPointName}");
        }
        else
        {
            Debug.LogWarning("No GameObject with 'Player' tag found in scene!");
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position, 0.5f);
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * 2f);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 1f);
    }
}
