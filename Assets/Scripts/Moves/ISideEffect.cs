using UnityEngine;

public interface ISideEffect
{
    void Apply(Minion target, Minion attacker);
    string GetDescription();
}
