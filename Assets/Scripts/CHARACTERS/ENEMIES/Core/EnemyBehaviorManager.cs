using UnityEngine;
using static EnemyStatsSO;

[DisallowMultipleComponent]
[RequireComponent(typeof(EnemyBaseController))]
public class EnemyBehaviorManager : MonoBehaviour
{
    private EnemyBaseController enemy;

    void Awake()
    {
        enemy = GetComponent<EnemyBaseController>();
    }

    void Start()
    {
        ApplyBehaviorTypeSettings();
    }

    private void ApplyBehaviorTypeSettings()
    {
        var stats = enemy.enemyStats;
        if (stats == null)
        {
            Debug.LogWarning($"[EnemyBehaviorManager] {enemy.name} has no EnemyStatsSO assigned!");
            return;
        }

        switch (stats.behaviorType)
        {
            case EnemyBehaviorType.Aggressive:
                enemy.canSearch = true;
                enemy.canPatrol = true;
                break;

            case EnemyBehaviorType.Fearful:
                enemy.canSearch = false;
                enemy.canPatrol = true;
                break;

            case EnemyBehaviorType.Defensive:
                enemy.canSearch = false;
                enemy.canPatrol = false;
                break;

            case EnemyBehaviorType.Neutral:
                enemy.canSearch = false;
                enemy.canPatrol = true;
                break;
        }
    }
}