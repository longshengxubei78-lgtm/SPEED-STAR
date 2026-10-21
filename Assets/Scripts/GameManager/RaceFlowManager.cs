using UnityEngine;
using XqLua;

public enum RacePhase
{
    Ready,      // レース開始前
    Racing,     // レース中
    Finished    // ゴール
}

public class RaceFlowManager : MonoBehaviour
{
    public IPublisher<RacePhase, RacePhase> _OnPhaseChaged => _onPhaseChanged;
    private Publisher<RacePhase, RacePhase> _onPhaseChanged = default;
    public RacePhase _currentPhase { get; private set; }
        = RacePhase.Ready;

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

        bool isValidTransition =
            (_currentPhase == RacePhase.Ready &&
             nextPhase == RacePhase.Racing)
            ||
            (_currentPhase == RacePhase.Racing &&
             nextPhase == RacePhase.Finished);

        if (!isValidTransition)
        {
            Debug.LogWarning(
                $"不正な状態遷移: {_currentPhase} -> {nextPhase}"
            );
            return;
        }

        RacePhase previousPhase = _currentPhase;
        _currentPhase = nextPhase;

        _onPhaseChanged?.Invoke(previousPhase, nextPhase);
    }
}