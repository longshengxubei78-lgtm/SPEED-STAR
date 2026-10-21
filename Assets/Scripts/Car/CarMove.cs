using UnityEngine;

public class CarMove
{
    private Rigidbody _myRigibody = default;

    private float _sideDecelerationRate = 0.95f;

    private float _driftDecelerationRate = 0.998f;

    private float _maxSpeed = 0.0f;

    // ===== 追加: 接地判定と姿勢安定化 =====
    private LayerMask _groundMask;
    private float _groundRayLength = 1.0f;   // 車の中心から路面までの距離＋余裕。車の大きさに合わせて調整
    private float _rollPitchDamping = 5.0f;  // 傾きの揺れを止める強さ（大きいほど早く止まる）
    private float _uprightStrength = 10.0f;  // 傾きを戻す力（大きすぎると再び揺れる）
    private bool _isGrounded = false;
    private Vector3 _groundNormal = Vector3.up;

    public Rigidbody GetRigidbody
    {
        get { return _myRigibody; }
    }
    public bool IsGrounded
    {
        get { return _isGrounded; }
    }

    // groundMask には「道路のレイヤーだけ」を渡す（車自身のレイヤーを含めない）
    public CarMove(Rigidbody rb, float maxSpeed, LayerMask groundMask)
    {
        _myRigibody = rb;
        _maxSpeed = maxSpeed;
        _groundMask = groundMask;
    }

    // FixedUpdate から呼ぶこと（Update から呼ぶとフレームレートで挙動が変わる）
    public void Tick()
    {
        CheckGround();
        StabilizeRotation();
        ReduceSidewaysVelocity();
    }

    // ===== 接地判定 =====
    private void CheckGround()
    {
        Transform myTransform = _myRigibody.transform;
        RaycastHit hit;

        _isGrounded = Physics.Raycast(
            _myRigibody.position, -myTransform.up, out hit,
            _groundRayLength, _groundMask);

        Vector3 targetNormal = _isGrounded ? hit.normal : Vector3.up;

        _groundNormal = Vector3.Slerp(_groundNormal, targetNormal, 0.2f);
    }

    // ===== 傾きの減衰と復元 =====
    private void StabilizeRotation()
    {
        Transform t = _myRigibody.transform;

        // ロール/ピッチ成分だけ減衰（ヨー＝ハンドルによる旋回は残す）
        Vector3 localAV = t.InverseTransformDirection(_myRigibody.angularVelocity);
        float k = Mathf.Exp(-_rollPitchDamping * Time.fixedDeltaTime);
        localAV.x *= k;
        localAV.z *= k;
        _myRigibody.angularVelocity = t.TransformDirection(localAV);

        // 路面の法線（空中なら世界の上）に向けて、弱い復元トルクをかける
        Vector3 axis = Vector3.Cross(t.up, _groundNormal);
        _myRigibody.AddTorque(axis * _uprightStrength, ForceMode.Acceleration);
    }

    public void Drift()
    {
        // 水平方向だけ減速する（Yまで減速すると落下がふわふわになる）
        Vector3 v = _myRigibody.linearVelocity;
        Vector3 horizontal = new Vector3(v.x, 0.0f, v.z);
        horizontal = Deceleration(_driftDecelerationRate, horizontal);

        _myRigibody.linearVelocity = new Vector3(horizontal.x, v.y, horizontal.z);
    }

    public void MovementSpeedLimit(Vector3 getVelocity)
    {
        // 水平方向だけ速度制限（落下速度やジャンプまで制限しない）
        Vector3 horizontal = new Vector3(getVelocity.x, 0.0f, getVelocity.z);
        horizontal = Vector3.ClampMagnitude(horizontal, _maxSpeed);

        _myRigibody.linearVelocity = new Vector3(horizontal.x, getVelocity.y, horizontal.z);
    }

    public void MoveForceCar(Vector3 forward, float _moveForce)
    {
        CheckGround();
        // 空中では加速しない
        if (!_isGrounded) return;

        ForceLerp(ref _moveForce);

        // 傾いていても、路面と平行な前方向に力をかける
        Vector3 flatForward = Vector3.ProjectOnPlane(forward, _groundNormal).normalized;
        _myRigibody.AddForce(flatForward * _moveForce);
    }

    private void ForceLerp(ref float force)
    {
        float currentSpeed = _myRigibody.linearVelocity.magnitude;

        currentSpeed = currentSpeed / _maxSpeed;

        if (currentSpeed > 0.5f)
        {
            force *= 0.7f;
        }
    }

    private void ReduceSidewaysVelocity()
    {
        // 空中では横滑り補正をしない
        CheckGround();
        if (!_isGrounded) return;

        Vector3 v = _myRigibody.linearVelocity;

        // 路面と平行な「右方向」を求め、その成分だけを減らす
        Vector3 right = Vector3.ProjectOnPlane(_myRigibody.transform.right, _groundNormal).normalized;
        float sidewaysSpeed = Vector3.Dot(v, right);

        // 60Hz基準の減衰率をFixedUpdateの間隔に合わせて補正
        float keep = Mathf.Pow(_sideDecelerationRate, Time.fixedDeltaTime * 60.0f);

        // 前後・上下の速度はそのまま、横成分だけを減らす
        v -= right * sidewaysSpeed * (1.0f - keep);

        _myRigibody.linearVelocity = v;
    }

    public void TurnCar(Quaternion rotation)
    {
        CheckGround();
        // 空中では旋回しない
        if (!_isGrounded) return;

        _myRigibody.MoveRotation(_myRigibody.rotation * rotation);
    }

    private Vector3 Deceleration(float decelerationRate, Vector3 linearVelocity)
    {
        linearVelocity *= decelerationRate;
        return linearVelocity;
    }
}
