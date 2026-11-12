using UnityEngine;

public class CarRotation
{
    // ===== 姿勢安定 =====
    private readonly float _rollPitchDamping = 5.0f;
    private readonly float _uprightStrength = 10.0f;


    /// ===== 傾きの減衰と復元 =====
    public void StabilizeRotation(Transform _transform,Rigidbody _rigidBody, Vector3 _groundNormal)
    {
        Vector3 localAV = _transform.InverseTransformDirection(_rigidBody.angularVelocity);
        float k = Mathf.Exp(-_rollPitchDamping * Time.fixedDeltaTime);
        localAV.x *= k;
        localAV.z *= k;
        _rigidBody.angularVelocity = _transform.TransformDirection(localAV);

        Vector3 axis = Vector3.Cross(_transform.up, _groundNormal);
        _rigidBody.AddTorque(axis * _uprightStrength, ForceMode.Acceleration);
    }
}
