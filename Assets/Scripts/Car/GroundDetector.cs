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
        Vector3 groundNormal = CheckGroundNormal();
        bool isGrounded = GroundSearch();
        float groundDistance = CheckGroundDistance();
        return new GroundInfo(groundNormal, isGrounded, groundDistance);
    }
    private Vector3 CheckGroundNormal()
    {
        bool _isGrounded = Physics.Raycast(
            _carTransform.position, -_carTransform.up, out RaycastHit hit,
             _groundMask);

        Vector3 targetNormal = Vector3.up;
        if (_isGrounded)
        {
            targetNormal = hit.normal;
        }

        return targetNormal;
    }
    private float CheckGroundDistance()
    {
        bool _isGrounded = Physics.Raycast(
            _carTransform.position, Vector3.down, out RaycastHit hit,
            _groundMask);

        float targetDistance = _groundRayLength;
        if (!_isGrounded)
        {
            return 0f;
        }
        targetDistance = hit.distance;

        return targetDistance;
    }
    private bool GroundSearch()
    {
        Vector3 origin = _carTransform.position + _carTransform.up * _startOffset;

        bool isGround = Physics.BoxCast(origin, _halfExtents, -_carTransform.up *0.1f, _carTransform.rotation, _maxDistance + _startOffset, _groundMask);
        return isGround;
    }
}