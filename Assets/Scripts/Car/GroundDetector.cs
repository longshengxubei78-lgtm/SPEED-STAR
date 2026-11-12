using UnityEngine;

public class GroundDetector
{
    private Transform _carTransform;
    private readonly float _groundRayLength = 1.0f;

    private float _maxDistance = 2f;
    private Vector3 _halfExtents = new Vector3(0.5f, 0.01f, 1f);
    private LayerMask _groundMask = default;
    public GroundDetector(Transform carTransform, LayerMask roadLayer)
    {
        _carTransform = carTransform;
        _groundMask = roadLayer;
    }
    private float _startOffset = 0.6f;
    public GroundInfo GetGroundInfo()
    {
        Vector3 groundNormal = CheckGround(Vector3.up);
        bool isGrounded = GroundSearch();
        return new GroundInfo(groundNormal, isGrounded);
    }
    // ===== 接地判定 =====
    public Vector3 CheckGround(Vector3 _groundNormal)
    {
        bool _isGrounded = Physics.Raycast(
            _carTransform.position, -_carTransform.up, out RaycastHit hit,
            _groundRayLength, _groundMask);

        Vector3 targetNormal = _isGrounded ? hit.normal : Vector3.up;
        _groundNormal = Vector3.Slerp(_groundNormal, targetNormal, 0.2f);

        return _groundNormal;
    }
    public bool GroundSearch()
    {
        Vector3 origin = _carTransform.position + _carTransform.up * _startOffset;

        bool isGround = Physics.BoxCast(origin, _halfExtents, -_carTransform.up *0.1f, _carTransform.rotation, _maxDistance + _startOffset);
        return isGround;
    }
    public float GroundDistance()
    {
        RaycastHit hit;
        bool isGroundHit=Physics.Raycast(_carTransform.position, Vector3.down,out hit, Mathf.Infinity);
        if(!isGroundHit)
        {
            return Mathf.Infinity;
        }
        float distance = hit.distance;

        return distance;
    }
}