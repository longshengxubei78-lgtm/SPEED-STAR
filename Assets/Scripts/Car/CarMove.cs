using UnityEngine;

public class CarMove
{
    // ===== 参照 =====
    private readonly Rigidbody _rigidbody;
    private readonly Transform _transform;
    private readonly SlopeDriveForceModifier _uphillDeceleration;

    // ===== 走行パラメータ =====
    private readonly float _moveForce;
    private readonly float _maxSpeed;
    private readonly float _rotateSpeed = 35f;
    private readonly float _sideDecelerationRate = 0.65f;
    private readonly float _driftDecelerationRate = 0.99f;
    private readonly float _driftTurnMultiplier = 1.5f;



    private Vector3 _groundNormal = Vector3.up;


    public CarMove(Rigidbody rb, Transform myTransform, float moveForce, float maxSpeed)
    {
        _rigidbody = rb;
        _transform = myTransform;
        _moveForce = moveForce;
        _maxSpeed = maxSpeed;

        _uphillDeceleration = new SlopeDriveForceModifier();
    }



    // 通常走行
    public void DriveUpdate(InputInfo input, GroundInfo groundInfo)
    {
        CheckGround(groundInfo.groundNormal);
        Accelerate(input.vertical);
        if (input.vertical != 0 || input.horizontal != 0)
        {
            Turn(input.horizontal, 1f);
        }
        LimitHorizontalSpeed();
        ReduceSidewaysVelocity();
    }

    // ドリフト走行
    public void DriftUpdate(InputInfo input, GroundInfo groundInfo)
    {
        CheckGround(groundInfo.groundNormal);
        ApplyDriftDeceleration();
        Accelerate(input.vertical);
        if (input.vertical != 0 || input.horizontal != 0)
        {
            Turn(input.horizontal, Mathf.Abs(input.horizontal) * _driftTurnMultiplier);
        }
        LimitHorizontalSpeed();
        ReduceSidewaysVelocity();
    }

    private void CheckGround(Vector3 targetNormal)
    {
        _groundNormal = Vector3.Slerp(_groundNormal, targetNormal, 0.2f);
    }

    private void Accelerate(float input)
    {
        float force = _uphillDeceleration.CalculateDecelerationRate(_transform) * _moveForce * input;

        // 速度が上がるほど加速を弱める
        if (_rigidbody.linearVelocity.magnitude / _maxSpeed > 0.5f)
        {
            force *= 0.7f;
        }

        Vector3 direction = Vector3.ProjectOnPlane(_transform.forward, _groundNormal).normalized;
        _rigidbody.AddForce(direction * force);
    }

    private void Turn(float horizontal, float multiplier)
    {
        float angle = horizontal * _rotateSpeed * Time.fixedDeltaTime * multiplier;
        _rigidbody.MoveRotation(_rigidbody.rotation * Quaternion.Euler(0f, angle, 0f));
    }

    /// <summary>
    /// ドリフト減速
    /// </summary>
    private void ApplyDriftDeceleration()
    {
        Vector3 v = _rigidbody.linearVelocity;
        Vector3 horizontal = new Vector3(v.x, 0f, v.z) * _driftDecelerationRate;
        _rigidbody.linearVelocity = new Vector3(horizontal.x, v.y, horizontal.z);
    }

   /// <summary>
   /// 速度制限(水平のみ)
   /// </summary>
    private void LimitHorizontalSpeed()
    {
        Vector3 velocity = _rigidbody.linearVelocity;
        Vector3 horizontal = Vector3.ClampMagnitude(new Vector3(velocity.x, 0f, velocity.z), _maxSpeed);
        _rigidbody.linearVelocity = new Vector3(horizontal.x, velocity.y, horizontal.z);
    }
    //↑
    //linearVelocityなので関数にする
    //引数と戻り値ををつけることで何をしているかがわかる。
    //rigidbody.linearVelocityをとって、加工しているところを関数にする。
    //↓


    /// <summary>
    /// 横滑り抑制
    /// </summary>
    private void ReduceSidewaysVelocity()
    {
        //二つの角度のない席の大きさで減速率を調整するべき
        //二つの角度がどれくらい似ているかが内積

        Vector3 v = _rigidbody.linearVelocity;
        Vector3 right = Vector3.ProjectOnPlane(_transform.right, _groundNormal).normalized;
        float sidewaysSpeed = Vector3.Dot(v, right);

        float keep = Mathf.Pow(_sideDecelerationRate, Time.fixedDeltaTime * 60f);
        _rigidbody.linearVelocity = v - right * sidewaysSpeed * (1f - keep);
    }
}