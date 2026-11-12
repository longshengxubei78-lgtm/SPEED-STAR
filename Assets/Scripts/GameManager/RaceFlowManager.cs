using UnityEngine;
using XqLua;

public enum RacePhase
{
    Ready,  
    Racing, 
    Finished
}

public class RaceFlowManager : MonoBehaviour
{
    public IPublisher<RacePhase, RacePhase> _OnPhaseChanged => _onPhaseChanged;
    private Publisher<RacePhase, RacePhase> _onPhaseChanged = default;
    public RacePhase _currentPhase { get; private set; }
        = RacePhase.Ready;


    [SerializeField]private CarController _playerCarController = default;
    private void Awake()
    {
        _onPhaseChanged = new Publisher<RacePhase, RacePhase>();
    }
    public void StartRace()
    {
        ChangePhase(RacePhase.Racing);
    }

    // ゴールラインから呼び出す
    public int PlayerRank { get; private set; }

    public void FinishRace(int playerRank)
    {
        if (_currentPhase != RacePhase.Racing)
            return;

        PlayerRank = playerRank;

        ChangePhase(RacePhase.Finished);
    }

    private void ChangePhase(RacePhase nextPhase)
    {
        if (_currentPhase == nextPhase)
            return;

        RacePhase previousPhase = _currentPhase;
        _currentPhase = nextPhase;

        _playerCarController.ApplyPhase(_currentPhase, nextPhase);
        _onPhaseChanged?.Invoke(previousPhase, nextPhase);
    }
}