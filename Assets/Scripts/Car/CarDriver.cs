using UnityEngine;

public class CarDriver
{
    // ===== 参照 =====
    private readonly Rigidbody _rigidbody;
    private readonly Transform _transform;
    private readonly UphillDeceleration _uphillDeceleration;
    private readonly GroundDetector _groundDetector = default;
    private readonly LayerMask _groundMask;

    // ===== 走行パラメータ =====
    private readonly float _moveForce;
    private readonly float _maxSpeed;
    private readonly float _rotateSpeed = 25f * 1.5f; // 元コードでは常に1.5倍が掛かっていた
    private readonly float _sideDecelerationRate = 0.95f;
    private readonly float _driftDecelerationRate = 0.998f;

    // ===== 姿勢安定 =====
    private readonly float _rollPitchDamping = 5.0f;
    private readonly float _uprightStrength = 10.0f;

    // ===== 状態 =====
    private bool _isGrounded;
    private Vector3 _groundNormal = Vector3.up;

    public bool IsGrounded => _isGrounded;
    public Rigidbody GetRigidbody => _rigidbody; // 外部で使っていなければ削除してOK

    public CarDriver(Rigidbody rb, Transform myTransform, float moveForce,
                     float maxSpeed, GroundDetector groundDetector)
    {
        _rigidbody = rb;
        _transform = myTransform;
        _moveForce = moveForce;
        _maxSpeed = maxSpeed;

        _groundDetector = groundDetector;
        _uphillDeceleration = new UphillDeceleration(_transform);
    }

    public void CarUpdate(float horizontal, float vertical)
    {

        CheckGround();

        float turnMultiplier = 1f;

        Accelerate(vertical);

        if (vertical != 0 || horizontal != 0)
        {
            Turn(horizontal, turnMultiplier);
        }

        LimitHorizontalSpeed();
        StabilizeRotation();
        ReduceSidewaysVelocity();
    }

    // 通常走行
    public void DriveUpdate(float horizontal, float vertical)
    {
        CheckGround();
        Accelerate(vertical);
        if (vertical != 0 || horizontal != 0)
            Turn(horizontal, 1f);
        ApplyCommonPhysics();
    }

    // ドリフト走行
    public void DriftUpdate(float horizontal, float vertical)
    {
        CheckGround();
        ApplyDriftDeceleration();
        Accelerate(vertical);
        if (vertical != 0 || horizontal != 0)
            Turn(horizontal, Mathf.Abs(horizontal) * 2f);
        ApplyCommonPhysics();

    }

    private void ApplyCommonPhysics()
    {
        LimitHorizontalSpeed();
        StabilizeRotation();
        ReduceSidewaysVelocity();
    }
    public float GetCurrentSpeed()
    {
        return _rigidbody.linearVelocity.magnitude;
    }

    // ===== 接地判定 =====
    private void CheckGround()
    {
        Vector3 targetNormal = _groundDetector.CheckGround(_groundNormal);

        if(targetNormal == Vector3.up)
        {
            _isGrounded = false;
        }
        else
        {
            _isGrounded = true;
        }
        _groundNormal = Vector3.Slerp(_groundNormal, targetNormal, 0.2f);
    }

    // ===== 加速 =====
    private void Accelerate(float input)
    {
        if (!_isGrounded) return; // 空中では加速しない

        float force = _uphillDeceleration.CalculateDecelerationRate() * _moveForce * input;

        // 速度が上がるほど加速を弱める
        if (_rigidbody.linearVelocity.magnitude / _maxSpeed > 0.5f)
        {
            force *= 0.7f;
        }

        Vector3 direction = Vector3.ProjectOnPlane(_transform.forward, _groundNormal).normalized;
        _rigidbody.AddForce(direction * force);
    }

    // ===== 旋回 =====
    private void Turn(float horizontal, float multiplier)
    {
        if (!_isGrounded) return; // 空中では旋回しない

        float angle = horizontal * _rotateSpeed * Time.fixedDeltaTime * multiplier;
        _rigidbody.MoveRotation(_rigidbody.rotation * Quaternion.Euler(0f, angle, 0f));
    }

    // ===== ドリフト減速(水平のみ) =====
    private void ApplyDriftDeceleration()
    {
        Vector3 v = _rigidbody.linearVelocity;
        Vector3 horizontal = new Vector3(v.x, 0f, v.z) * _driftDecelerationRate;
        _rigidbody.linearVelocity = new Vector3(horizontal.x, v.y, horizontal.z);
    }

    // ===== 速度制限(水平のみ) =====
    private void LimitHorizontalSpeed()
    {
        Vector3 velocity = _rigidbody.linearVelocity;
        Vector3 horizontal = Vector3.ClampMagnitude(new Vector3(velocity.x, 0f, velocity.z), _maxSpeed);
        _rigidbody.linearVelocity = new Vector3(horizontal.x, velocity.y, horizontal.z);
    }

    // ===== 横滑り抑制 =====
    private void ReduceSidewaysVelocity()
    {
        if (!_isGrounded) return;

        Vector3 v = _rigidbody.linearVelocity;
        Vector3 right = Vector3.ProjectOnPlane(_transform.right, _groundNormal).normalized;
        float sidewaysSpeed = Vector3.Dot(v, right);

        float keep = Mathf.Pow(_sideDecelerationRate, Time.fixedDeltaTime * 60f);
        _rigidbody.linearVelocity = v - right * sidewaysSpeed * (1f - keep);
    }

    /// ===== 傾きの減衰と復元 =====
    private void StabilizeRotation()
    {
        Vector3 localAV = _transform.InverseTransformDirection(_rigidbody.angularVelocity);
        float k = Mathf.Exp(-_rollPitchDamping * Time.fixedDeltaTime);
        localAV.x *= k;
        localAV.z *= k;
        _rigidbody.angularVelocity = _transform.TransformDirection(localAV);

        Vector3 axis = Vector3.Cross(_transform.up, _groundNormal);
        _rigidbody.AddTorque(axis * _uprightStrength, ForceMode.Acceleration);
    }
}