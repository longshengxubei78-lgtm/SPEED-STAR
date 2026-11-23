using UnityEngine;

public class CarHover
{
    private const float RayStartOffset = 0.5f;

    private readonly Rigidbody _rb;
    private readonly Transform _transform;
    private readonly Transform[] _anchors;
    private readonly LayerMask _roadLayer;
    private readonly float _restLength;
    private readonly float _springK;
    private readonly float _damperC;

    public bool IsGrounded { get; private set; }

    public CarHover(Rigidbody rb, Transform myTransform, Transform[] anchors, LayerMask roadLayer,
                    float hoverHeight, float maxLoadRatio = 3f, float dampingRatio = 0.8f)
    {
        _rb = rb;
        _transform = myTransform;
        _anchors = anchors ?? System.Array.Empty<Transform>();
        _roadLayer = roadLayer;

        if (_anchors.Length == 0)
        {
            Debug.LogError("CarHover: _hoverAnchors が空です");
            return;
        }

        _restLength = hoverHeight * maxLoadRatio / (maxLoadRatio - 1f);
        float restCompression = _restLength - hoverHeight;
        float weightPerAnchor = rb.mass * Mathf.Abs(Physics.gravity.y) / _anchors.Length;
        _springK = weightPerAnchor / restCompression;
        _damperC = 2f * dampingRatio * Mathf.Sqrt(_springK * rb.mass / _anchors.Length);
    }

    public void FixedUpdate()
    {
        Vector3 up = _transform.up;
        int hitCount = 0;

        foreach (Transform anchor in _anchors)
        {
            Vector3 origin = anchor.position + up * RayStartOffset;
            if (!Physics.Raycast(origin, -up, out RaycastHit hit, RayStartOffset + _restLength,
                                 _roadLayer, QueryTriggerInteraction.Ignore))
            {
                continue;
            }
            hitCount++;

            float gap = hit.distance - RayStartOffset; // アンカーと路面の距離
            float compression = _restLength - gap;
            float velAlongUp = Vector3.Dot(_rb.GetPointVelocity(anchor.position), up);
            float force = Mathf.Max(0f, compression * _springK - velAlongUp * _damperC);

            _rb.AddForceAtPosition(up * force, anchor.position);
        }

        IsGrounded = hitCount > 0;
    }
}