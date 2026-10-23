using UnityEngine;

public class CPUMoveTargetSearch
{
    private float _detectionDistance = 100f;
    private Transform _cpuTransform = default;
    private Transform _playerTransform = default;
    private GameObject _target = default;
    private Rigidbody _myRigidbody = default;
    private InputPresenter _inputPresenter = default;

    private RaycastHit _forwardWall = default;
    private RaycastHit _rightWall = default;
    private RaycastHit _leftWall = default;
    private bool _forwardHit = default;
    private bool _rightHit = default;
    private bool _leftHit = default;


    private bool _isDrifting = default;
    private float _driftTimer = 0f;

private const float _startSpeed = 15f;     // 15 → 10
private const float _endSpeed = 8f;        // 8 → 5
private const float _startAngle = 30f;     // 20 → 12
private const float _endAngle = 15f;        // 15 → 7
private const float _minDriftTime = 0.3f; // 0.3 → 0.35

private const float _wallMargin = 1f;      // 1 → 4
private const float _wallAvoidDistance = 10f;
private const float _forwardWallAvoidDistance = 15f;
private const float _maxSidewaysSpeed = 120f; // 120 → 12
    private const float _minPlayerDistance = 50f;
    private const float _maxPlayerDistance = 120f;

    private float _changepointdistance = 12f;

    private int _wallMask;                        // 壁のレイヤーだけ（車・トリガーを含めない）
    private float _halfWidth = 1.0f;              // 車の半幅。実寸に合わせる
    private const float _diagAngle = 40f;         // 斜めレイの角度
    private const float _speedAvoidFactor = 0.5f; // 速度×この値(m)を回避開始距離の下限にする

    private RaycastHit _rightDiagWall = default;
    private RaycastHit _leftDiagWall = default;
    private bool _rightDiagHit = default;
    private bool _leftDiagHit = default;
    public CPUMoveTargetSearch(Transform transform, GameObject fastTarget, InputPresenter inputPresenter,
        Rigidbody rigidbody, Transform playerTransform, int wallMask)
    {
        _cpuTransform = transform;
        _target = fastTarget;
        _inputPresenter = inputPresenter;
        _myRigidbody = rigidbody;
        _playerTransform = playerTransform;
        _wallMask = wallMask;   // 例: LayerMask.GetMask("Wall")
    }

    public void FixedTick()
    {
        if (_target == null) return;

        ChangeTarget();
        UpdateWallDetection(_detectionDistance);

        SetInput();
    }

    public void ChangeTarget()
    {
        float distance = Vector3.Distance(_target.transform.position, _cpuTransform.position);

        if (distance < _changepointdistance)
        {
            if (_target.TryGetComponent<WayPointsSetting>(out WayPointsSetting wayPoints))
            {
                GameObject next = wayPoints.OnGetWayPoint();
                if (next != null)
                {
                    _target = next;
                }
            }
        }
    }


    private void SetInput()
    {
        float horizontal = GetHorizontal();
        float vertical = GetVertical() * GetCornerBrake();
        bool drift = ShouldDrift();
        _inputPresenter.SetMoveValue(horizontal, 1f);
        _inputPresenter.SetDriftValue(false);
    }

    private float GetVertical()
    {
        Vector3 toPlayer =
            _playerTransform.position - _cpuTransform.position;

        // CPUの進行方向を基準に、プレイヤーとの前後距離を取得
        float playerForwardDistance =
            Vector3.Dot(toPlayer, _cpuTransform.forward);

        // プレイヤーが前にいる場合は常に最大入力
        if (playerForwardDistance >= 0f)
        {
            return 1f;
        }

        // プレイヤーが後ろにいる距離
        float behindDistance = -playerForwardDistance;

        // min未満なら減速しない
        if (behindDistance < _minPlayerDistance)
        {
            return 1f;
        }

        // min〜maxの範囲で徐々に減速
        float t = Mathf.InverseLerp(
            _minPlayerDistance,
            _maxPlayerDistance,
            behindDistance
        );

        return Mathf.Lerp(1f, 0f, t);
    }
    /// <summary>
    /// ===== ドリフト判断 =====
    /// </summary>
    /// <returns></returns>
    private bool ShouldDrift()
    {
        float speed = GetCurrentSpeed();
        float cornerAngle = GetCornerAngle();

        if (_isDrifting)
        {
            _driftTimer += Time.fixedDeltaTime;

            // 壁が近い、または横滑りが大きい場合は即座に終了
            bool emergencyEnd =
                IsOuterWallDanger() ||
                Mathf.Abs(GetSidewaysSpeed()) > _maxSidewaysSpeed;

            if (emergencyEnd)
            {
                _isDrifting = false;
                return false;
            }

            // 通常の終了判定には最低継続時間を適用
            if (_driftTimer < _minDriftTime)
            {
                return true;
            }

            bool shouldEnd =
                speed < _endSpeed ||
                cornerAngle < _endAngle;

            if (shouldEnd)
            {
                _isDrifting = false;
            }

            return _isDrifting;
        }

        // ドリフト開始条件を緩和
        bool shouldStart =
            speed > _startSpeed &&
            cornerAngle > _startAngle &&
            !IsOuterWallDanger();

        if (shouldStart)
        {
            _isDrifting = true;
            _driftTimer = 0f;
        }

        return _isDrifting;
    }

    /// <summary>
    /// 現在速度
    /// </summary>
    /// <returns></returns>
    public float GetCurrentSpeed()
    {
        return _myRigidbody.linearVelocity.magnitude;
    }

    /// <summary>
    /// 目標方向への左右入力値（-1〜1）
    /// </summary>
    /// <returns></returns>
    public float GetHorizontal()
    {
        float leftWallDistance = GetLeftWallDistance();
        float rightWallDistance = GetRightWallDistance();
        float forwardWallDistance = GetForwardWallDistance();

        float speed = GetCurrentSpeed();
        float avoidDistance = Mathf.Max(_wallAvoidDistance, speed * _speedAvoidFactor);
        float forwardAvoidDistance = Mathf.Max(_forwardWallAvoidDistance, speed * 1.0f);

        // 目標方向への基本舵。水平面での角度差を 30° で 1.0 とする
        Vector3 toTarget = Vector3.ProjectOnPlane(_target.transform.position - _cpuTransform.position, Vector3.up);
        Vector3 fwd = Vector3.ProjectOnPlane(_cpuTransform.forward, Vector3.up);
        float horizontal = Mathf.Clamp(Vector3.SignedAngle(fwd, toTarget, Vector3.up) / 30f, -1f, 1f);

        // 左の壁が近いほど右へ（壁が無ければ距離が検知距離なので条件に入らない）
        if (leftWallDistance < avoidDistance)
        {
            float t = 1f - leftWallDistance / avoidDistance;
            horizontal += Mathf.Lerp(0.25f, 1f, t);
        }

        // 右の壁が近いほど左へ
        if (rightWallDistance < avoidDistance)
        {
            float t = 1f - rightWallDistance / avoidDistance;
            horizontal -= Mathf.Lerp(0.25f, 1f, t);
        }

        // 正面の壁が近い場合、広い方向へ
        if (_forwardHit && forwardWallDistance < forwardAvoidDistance)
        {
            if (leftWallDistance > rightWallDistance + 2f)
            {
                horizontal = Mathf.Min(horizontal, -0.85f);
            }
            else if (rightWallDistance > leftWallDistance + 2f)
            {
                horizontal = Mathf.Max(horizontal, 0.85f);
            }
            else
            {
                horizontal = horizontal >= 0f ? -0.85f : 0.85f;
            }
        }

        return Mathf.Clamp(horizontal, -1f, 1f);
    }

    /// <summary>
    /// 目標との角度
    /// </summary>
    /// <returns></returns>
    public float GetTargetAngle()
    {
        Vector3 targetDirection =
            _target.transform.position - _cpuTransform.position;

        return Vector3.Angle(_cpuTransform.forward, targetDirection);
    }

    /// <summary>
    /// 次の目標との角度
    /// </summary>
    /// <returns></returns>
    public float GetNextTargetAngle()
    {
        if (!TryGetNextTarget(out GameObject nextTarget))
            return 0f;

        Vector3 currentDirection =
            (_target.transform.position - _cpuTransform.position).normalized;

        Vector3 nextDirection =
            (nextTarget.transform.position - _target.transform.position).normalized;

        return Vector3.Angle(currentDirection, nextDirection);
    }

    /// <summary>
    /// 壁までの距離（壁がなければ検知距離を返す）
    /// </summary>
    /// <returns></returns>
    public float GetLeftWallDistance()
    {
        float side = _leftHit ? _leftWall.distance : _detectionDistance;
        float diag = _leftDiagHit
            ? _leftDiagWall.distance * Mathf.Sin(_diagAngle * Mathf.Deg2Rad)
            : _detectionDistance;
        return Mathf.Max(0f, Mathf.Min(side, diag) - _halfWidth);
    }

    public float GetRightWallDistance()
    {
        float side = _rightHit ? _rightWall.distance : _detectionDistance;
        float diag = _rightDiagHit
            ? _rightDiagWall.distance * Mathf.Sin(_diagAngle * Mathf.Deg2Rad)
            : _detectionDistance;
        return Mathf.Max(0f, Mathf.Min(side, diag) - _halfWidth);
    }

    public float GetForwardWallDistance()
    {
        return _forwardHit ? _forwardWall.distance : _detectionDistance;
    }

    /// <summary>
    /// 現在の横方向速度（+ 右へ滑っている / - 左へ滑っている）
    /// </summary>
    /// <returns></returns>
    public float GetSidewaysSpeed()
    {
        return Vector3.Dot(_myRigidbody.linearVelocity, _cpuTransform.right);
    }

    /// <summary>
    /// カーブ方向（+1 右 / -1 左 / 0 直線）
    /// </summary>
    /// <returns></returns>
    public float GetCurveDirection()
    {
        if (!TryGetNextTarget(out GameObject nextTarget))
            return 0f;

        Vector3 currentDirection = (_target.transform.position - _cpuTransform.position).normalized;
        Vector3 nextDirection = (nextTarget.transform.position - _target.transform.position).normalized;

        Vector3 cross = Vector3.Cross(currentDirection, nextDirection);

        if (Mathf.Abs(cross.y) < 0.01f) return 0f;   // ほぼ直線
        return Mathf.Sign(cross.y);
    }

    // 直線（0）のときは外側の壁を気にしない
    private bool IsOuterWallDanger()
    {
        float dir = GetCurveDirection();
        if (dir == 0f) return false;

        float outer = dir > 0 ? GetLeftWallDistance() : GetRightWallDistance();
        return outer < _wallMargin;
    }



    private bool TryGetNextTarget(out GameObject nextTarget)
    {
        nextTarget = null;

        if (!_target.TryGetComponent<WayPointsSetting>(out WayPointsSetting setting))
            return false;

        nextTarget = setting.OnGetWayPoint();
        return nextTarget != null;
    }

    private void UpdateWallDetection(float detectionDistance)
    {
        Vector3 origin = _cpuTransform.position;

        Vector3 fwd = Vector3.ProjectOnPlane(_cpuTransform.forward, Vector3.up).normalized;
        Vector3 right = Vector3.ProjectOnPlane(_cpuTransform.right, Vector3.up).normalized;
        Vector3 rightDiag = Quaternion.AngleAxis(_diagAngle, Vector3.up) * fwd;
        Vector3 leftDiag = Quaternion.AngleAxis(-_diagAngle, Vector3.up) * fwd;

        QueryTriggerInteraction ignore = QueryTriggerInteraction.Ignore;

        _forwardHit = Physics.Raycast(origin, fwd, out _forwardWall, detectionDistance, _wallMask, ignore);
        _rightHit = Physics.Raycast(origin, right, out _rightWall, detectionDistance, _wallMask, ignore);
        _leftHit = Physics.Raycast(origin, -right, out _leftWall, detectionDistance, _wallMask, ignore);
        _rightDiagHit = Physics.Raycast(origin, rightDiag, out _rightDiagWall, detectionDistance, _wallMask, ignore);
        _leftDiagHit = Physics.Raycast(origin, leftDiag, out _leftDiagWall, detectionDistance, _wallMask, ignore);
    }

    // 近いときは今の目標、遠いときは次のカーブを重く見る
    private float GetCornerAngle()
    {
        float distance = Vector3.Distance(_target.transform.position, _cpuTransform.position);
        float nextWeight = distance < _changepointdistance * 3f ? 1f : 0.5f;
        return Mathf.Max(GetTargetAngle(), GetNextTargetAngle() * nextWeight);
    }
    private const float _brakeStartAngle = 25f;  // このカーブ角度から減速を検討
    private const float _fastCornerSpeed = 30f;  // ゆるいカーブで許す速度（スケールに合わせる）
    private const float _sharpCornerSpeed = 12f; // 急カーブで許す速度

    private float GetCornerBrake()
    {
        float angle = GetCornerAngle();
        if (angle < _brakeStartAngle) return 1f;

        float t = Mathf.InverseLerp(_brakeStartAngle, 90f, angle);
        float targetSpeed = Mathf.Lerp(_fastCornerSpeed, _sharpCornerSpeed, t);

        // 目標速度を3超えるごとに加速入力を絞り、超えたら0（アクセスオフ）
        return Mathf.Clamp01(1f - (GetCurrentSpeed() - targetSpeed) / 3f);
    }

}