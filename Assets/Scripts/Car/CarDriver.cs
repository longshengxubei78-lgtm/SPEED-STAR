using UnityEngine;

public class CarDriver
{
    // ===== 参照 =====
    private readonly Rigidbody _rigidbody;
    private readonly Transform _transform;
    private readonly UphillDeceleration _uphillDeceleration;

    // ===== 走行パラメータ =====
    private readonly float _moveForce;
    private readonly float _maxSpeed;
    private readonly float _rotateSpeed = 35f;
    private readonly float _sideDecelerationRate = 0.85f;
    private readonly float _driftDecelerationRate = 0.90f;



    // ===== 状態 =====
    private bool _isGrounded;
    private Vector3 _groundNormal = Vector3.up;


    public CarDriver(Rigidbody rb, Transform myTransform, float moveForce, float maxSpeed)
    {
        _rigidbody = rb;
        _transform = myTransform;
        _moveForce = moveForce;
        _maxSpeed = maxSpeed;

        _uphillDeceleration = new UphillDeceleration();
    }



    // 通常走行
    public void DriveUpdate(InputInfo input, GroundInfo groundInfo)
    {
        CheckGround(groundInfo.groundNormal);
        Accelerate(input.vertical);
        if (input.vertical != 0 || input.horizontal != 0)
            Turn(input.horizontal, 1f);
        ApplyCommonPhysics();
    }

    // ドリフト走行
    public void DriftUpdate(InputInfo input, GroundInfo groundInfo)
    {
        CheckGround(groundInfo.groundNormal);
        ApplyDriftDeceleration();
        Accelerate(input.vertical);
        if (input.vertical != 0 || input.horizontal != 0)
            Turn(input.horizontal, Mathf.Abs(input.horizontal) * 2f);
        ApplyCommonPhysics();

    }

    private void ApplyCommonPhysics()
    {
        LimitHorizontalSpeed();
        ReduceSidewaysVelocity();
    }
    public float GetCurrentSpeed()
    {
        return _rigidbody.linearVelocity.magnitude;
    }

    // ===== 接地判定 =====
    private void CheckGround(Vector3 targetNormal)
    {

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

        float force = _uphillDeceleration.CalculateDecelerationRate(_transform) * _moveForce * input;

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
        if (!_isGrounded) return; 

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
}