using UnityEngine;
using TMPro;
public class SpeedMeter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _speedMeter;

   
    public void SetMeterValue(float speedValue)
    {
        speedValue *= 3.6f;
        speedValue = Mathf.FloorToInt(speedValue);
        _speedMeter.text = speedValue.ToString()+"/kmh";
    }
}
