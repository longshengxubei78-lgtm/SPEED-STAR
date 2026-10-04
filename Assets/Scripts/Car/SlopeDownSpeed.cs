using UnityEngine;

public class UphillDeceleration
{
    private SlopeDetector _detector;
    private Transform _carTransform;
    public UphillDeceleration(Transform carTransform)
    {
        _detector = new SlopeDetector();
        _carTransform = carTransform;
    }
    public float CalculateDecelerationRate()
    {

        float slopeCalculate=_detector.CalculateSlopeFactor(_carTransform);

        slopeCalculate -= 1f;
        float rotate = Mathf.Abs(slopeCalculate);

        return rotate;
    }
}
