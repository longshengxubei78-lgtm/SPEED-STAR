using System;
using TMPro;
using UnityEngine;
using XqLua;

public class ResultManager : MonoBehaviour , IDisposable
{
    [SerializeField] private Canvas _resultCanvas = default;
    [SerializeField] private TextMeshProUGUI _rankText = default;
    [SerializeField] private RaceFlowManager _raceFlowManager = default;
    private IDisposableSubscription _subscription = default;
    void Start()
    {
        _resultCanvas.gameObject.SetActive(false);
        _subscription = _raceFlowManager._OnPhaseChanged.Subscribe(PopResult);
    }
    private void PopResult(RacePhase phase,RacePhase nextPhase)
    {
        switch (nextPhase)
        {
            case RacePhase.Finished:
                _resultCanvas.gameObject.SetActive(true);
                _rankText.text = _raceFlowManager.PlayerRank.ToString()+" th";
                break;
        }
    }

    public void Dispose()
    {
        _subscription.Dispose();
    }
}
