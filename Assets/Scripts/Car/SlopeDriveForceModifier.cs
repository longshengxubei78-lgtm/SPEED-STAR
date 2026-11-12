using UnityEngine;

public class SlopeDriveForceModifier
{
    public float CalculateDecelerationRate(Transform carTransform)
    {

        float slopeCalculate=CalculateSlopeFactor(carTransform);

        slopeCalculate -= 1f;
        float rotate = Mathf.Abs(slopeCalculate);

        return rotate;
    }
    public float CalculateSlopeFactor(Transform carTransform)
    {
        float f = Vector3.Dot(carTransform.forward, Vector3.up);
        return f;
    }
}
