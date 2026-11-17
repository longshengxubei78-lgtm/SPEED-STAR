using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private CarController _carController = default;
    [SerializeField]private RaceFlowManager _raceFlowManager = default;
    private void FixedUpdate()
    {
        switch (_raceFlowManager._currentPhase)
        {
            case RacePhase.Ready:
                break;
            case RacePhase.Racing:
                _carController.FixedTick();
                break;
            case RacePhase.Finished:
                break;
        }
    }
}