using System.Collections.Generic;
using UnityEngine;

public class GoalArea : MonoBehaviour
{
    [SerializeField] private RaceFlowManager _raceFlowManager = default;

    private int _cpuFinishCount;

    private readonly HashSet<CPUStateMachine> _finishedCPUs
        = new HashSet<CPUStateMachine>();

    private void OnTriggerEnter(Collider other)
    {
        CPUStateMachine cpu =
            other.GetComponentInParent<CPUStateMachine>();

        if (cpu != null)
        {
            if (_finishedCPUs.Add(cpu))
            {
                _cpuFinishCount++;
            }

            return;
        }

        CarController player =
            other.GetComponentInParent<CarController>();

        if (player == null)
            return;
        int playerRank = _cpuFinishCount + 1;

        _raceFlowManager.FinishRace(playerRank);
    }
}
