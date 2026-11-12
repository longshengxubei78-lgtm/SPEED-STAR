using UnityEngine;
using TMPro;
public class SpeedMeter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _speedMeter = default;
    private IPlayerStatus _playerStatus = default;

    public void Initialize(IPlayerStatus playerStatus)
    {
        _playerStatus = playerStatus;
    }

    private void FixedUpdate()
    {
        SetMeterValue(_playerStatus.CurrentSpeed);
    }
    public void SetMeterValue(float speedValue)
    {
        speedValue *= 3.6f;
        speedValue = Mathf.FloorToInt(speedValue);
        _speedMeter.text = speedValue.ToString()+"/kmh";
    }
}
