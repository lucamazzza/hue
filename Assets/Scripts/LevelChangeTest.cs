using UnityEngine;
using UnityEngine.SceneManagement;

public class TestLevelChanger : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            SceneManager.LoadScene("LevelSea");
        }
    }
}