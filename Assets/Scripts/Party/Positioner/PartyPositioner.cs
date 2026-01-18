using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[DefaultExecutionOrder(-200)]
public abstract class PartyPositioner : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private float radius = 5f;
    [SerializeField, Range(0, 360)] private float arcAngle = 180f;

    [Header("UI Configuration")]
    [SerializeField] protected Transform uiContainer;
    [SerializeField] protected MinionHealthDisplay healthUiPrefab;

    protected List<MinionStatsSO> _partyMembers = new List<MinionStatsSO>();

    protected virtual void Start()
    {
        InitializeParty();
        PositionMinions();
    }

    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;

        int count = (_partyMembers != null && _partyMembers.Count > 0) ? _partyMembers.Count : 4;

        float angleStep = arcAngle / (count + 1);
        float currentYRotation = transform.eulerAngles.y;
        float startOffset = -arcAngle / 2f;

        for (int i = 0; i < count; i++)
        {
            float deg = currentYRotation + startOffset + (angleStep * (i + 1));
            Vector3 pos = GetPositionOnCircle(deg);

            Gizmos.DrawWireSphere(pos, 0.5f);

            Gizmos.color = Color.red;
            Gizmos.DrawRay(pos, Vector3.left * 2f);
            Gizmos.color = Color.green;
        }
    }

    protected abstract void InitializeParty();
    protected abstract void ConfigureMinion(GameObject minionObj, Minion minion, int index);


    private void PositionMinions()
    {
        if (_partyMembers == null || _partyMembers.Count == 0)
            return;

        float angleStep = arcAngle / (_partyMembers.Count + 1);
        float currentYRotation = transform.eulerAngles.y;
        float startOffset = -arcAngle / 2f;

        for (int i = 0; i < _partyMembers.Count; i++)
        {
            float relativeAngle = startOffset + (angleStep * (i + 1));
            float finalAngle = currentYRotation + relativeAngle;

            Vector3 spawnPosition = GetPositionOnCircle(finalAngle);
            GameObject prefab = _partyMembers[i].minionPrefab;

            if (prefab != null)
            {
                Quaternion facingRotation = Quaternion.identity;
                GameObject gameObject = Instantiate(prefab, spawnPosition, facingRotation);

                Minion minion = gameObject.AddComponent<Minion>();
                gameObject.AddComponent<MeshCollider>();
                gameObject.AddComponent<MinionAttack>();
                gameObject.AddComponent<StatusEffectManager>();
                
                ConfigureMinion(gameObject, minion, i);

                if (uiContainer != null && healthUiPrefab != null)
                {
                    MinionHealthDisplay healthDisplay = Instantiate(healthUiPrefab, uiContainer);
                    healthDisplay.Initialize(minion, _partyMembers[i].minionIcon);
                }

                Debug.Log($"Spawned {gameObject.name}");
            }
        }
    }

    private Vector3 GetPositionOnCircle(float degrees)
    {
        float radians = degrees * Mathf.Deg2Rad;

        float x = transform.position.x + radius * Mathf.Sin(radians);
        float y = transform.position.y;
        float z = transform.position.z + radius * Mathf.Cos(radians);

        return new Vector3(x, y, z);
    }
}
