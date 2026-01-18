using System.Collections;
using UnityEngine;

public class MinionAttack : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 5.0f;
    [SerializeField] private float _attackDistance = 1.5f;
    [SerializeField] private float _attackDuration = 0.3f;
    
    private Minion _minion;
    private Vector3 _originalPosition;
    private Quaternion _originalRotation;
    private bool _isAttacking = false;
    private float _moveAttack;

    public bool IsAttacking => _isAttacking;

    void Start()
    {
        _minion = GetComponent<Minion>();
        _originalPosition = transform.position;
        _originalRotation = transform.rotation;
        _moveAttack = _minion.availableMoves[0].attack;
    }

    public void ExecuteAttack(Minion target)
    {
        if (_isAttacking || target == null || !target.IsAlive)
            return;
        StartCoroutine(AttackSequence(target));
    }

    private IEnumerator AttackSequence(Minion target)
    {
        _isAttacking = true;
        _originalPosition = transform.position;
        _originalRotation = transform.rotation;
        Vector3 directionToTarget = (target.transform.position - transform.position).normalized;
        Vector3 attackPosition = target.transform.position - directionToTarget * _attackDistance;
        yield return StartCoroutine(MoveToPosition(attackPosition));
        transform.LookAt(target.transform);
        target.TakeDamage(_minion.AttackDamage + _moveAttack);
        yield return new WaitForSeconds(_attackDuration);
        yield return StartCoroutine(MoveToPosition(_originalPosition));
        transform.rotation = _originalRotation;
        _isAttacking = false;
    }

    private IEnumerator MoveToPosition(Vector3 targetPosition)
    {
        while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, _moveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPosition;
    }
}
