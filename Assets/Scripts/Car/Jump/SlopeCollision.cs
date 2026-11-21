using UnityEngine;

public class SlopeCollision : MonoBehaviour
{
    [SerializeField] private Rigidbody _rb = default;
    [SerializeField] private float _maxSlopeAngle = 45f;
    [SerializeField] private float _slopeSpeedKeep = 0.5f;


    private void OnCollisionEnter(Collision collision)
    {
        Vector3 normal = Vector3.zero;
        for (int i = 0; i < collision.contactCount; i++)
        {
            ContactPoint contact = collision.GetContact(i);
            normal += contact.normal;
        }
        normal.Normalize();
        KeepSpeedAlongSlope(normal);
    }
    Vector3 _preStepVelocity;
    public void KeepSpeedAlongSlope(Vector3 n)
    {
        if (Vector3.Angle(n, Vector3.up) > _maxSlopeAngle) 
        {
            // 壁は対象外
            return;
        } 
        if (Vector3.Dot(_preStepVelocity, n) >= 0f)
        {
            // 離れる向きは無視
            return;
        }
        Vector3 along = Vector3.ProjectOnPlane(_preStepVelocity, n);
        if (along.sqrMagnitude < 0.01f)
        {
            return;
        }
        _rb.linearVelocity = along.normalized * (_preStepVelocity.magnitude * _slopeSpeedKeep);
    }
}
