using UnityEngine;

public class SlopeDetector
{
    public float CalculateSlopeFactor(Transform carTransform)
    {
        float f = Vector3.Dot(carTransform.forward, Vector3.up);
        return f;
    }
}
