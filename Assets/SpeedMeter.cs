using UnityEngine;
using TMPro;
public class SpeedMeter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _speedMeter;


    public void SetMeterValue(float speedValue)
    {
        speedValue *= 3.6f;
        _speedMeter.text = speedValue.ToString();
    }
}
