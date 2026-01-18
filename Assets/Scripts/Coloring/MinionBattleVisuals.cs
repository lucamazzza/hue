using UnityEngine;

public class MinionBattleVisuals : MonoBehaviour
{
    [Header("Riferimento Dati")]
    public MinionStatsSO stats; 

    void Start()
    {
        ApplyCustomTexture();
    }

    public void ApplyCustomTexture()
    {
        if (stats == null || stats.savedCustomTexture == null) return;

        Transform drawingTransform = transform.Find("Drawing");
        
        Renderer rend = (drawingTransform != null) 
                        ? drawingTransform.GetComponent<Renderer>() 
                        : GetComponentInChildren<Renderer>();

        if (rend != null)
        {
            rend.material.mainTexture = stats.savedCustomTexture;
        }
    }
}