using UnityEngine;

public class CarHover
{
    private readonly Rigidbody _rb;
    private readonly Transform _transform;
    private readonly Transform[] _anchors;
    private readonly LayerMask _roadLayer;

    private readonly float _restLength;  // レイの長さ(ここまで近づくと押し返し始める)
    private readonly float _springK;     // バネの強さ
    private readonly float _damperC;     // 揺れを止める力

    public bool IsGrounded { get; private set; }

    public CarHover(Rigidbody rb, Transform myTransform, Transform[] anchors, LayerMask roadLayer,
                    float hoverHeight = 0.15f,   // 止まっているときに浮く高さ
                    float maxLoadRatio = 3f,     // バネの上限は車重の何倍か
                    float dampingRatio = 0.8f)   // 0.6〜1。大きいほど揺れにくい
    {
        _rb = rb;
        _transform = myTransform;
        _anchors = anchors;
        _roadLayer = roadLayer;

        _restLength = hoverHeight * maxLoadRatio / (maxLoadRatio - 1f);
        float restCompression = _restLength - hoverHeight;

        float weightPerAnchor = rb.mass * Mathf.Abs(Physics.gravity.y) / anchors.Length;
        _springK = weightPerAnchor / restCompression;
        _damperC = 2f * dampingRatio * Mathf.Sqrt(_springK * rb.mass / anchors.Length);
    }
    public void FixedUpdate()
    {
        int hitCount = 0;
        foreach (Transform anchor in _anchors)
        {
            Vector3 origin = anchor.position;
            if (!Physics.Raycast(origin, -_transform.up, out RaycastHit hit,
                                 _restLength, _roadLayer, QueryTriggerInteraction.Ignore))
            {
                continue;
            }
            hitCount++;

            float compression = _restLength - hit.distance;
            float velAlongUp = Vector3.Dot(_rb.GetPointVelocity(origin), _transform.up);
            float force = Mathf.Max(0f, compression * _springK - velAlongUp * _damperC);

            _rb.AddForceAtPosition(_transform.up * force, origin);
        }
        IsGrounded = hitCount > 0;
    }
}