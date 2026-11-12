
using UnityEngine;
using UnityEngine.InputSystem;

public class CarSpinAction : MonoBehaviour
{
    [SerializeField] private PlayerInput _playerInput = default;
    [SerializeField] private Rigidbody _myRigidbody = default;

    [SerializeField] private float _spinInputWindow = 0.5f;
    [SerializeField] private float _spinDuration = 0.6f;

    private Quaternion _firstRotation = Quaternion.identity;

    private bool _isSpin = false;
    private float _spinElapsedTime = 0f;

    private readonly Timer _doubleTapTimer = new Timer();

    private void OnEnable()
    {
        _playerInput.actions["Drift"].started += SpinInput;
    }

    private void OnDisable()
    {
        _playerInput.actions["Drift"].started -= SpinInput;
    }

    private void FixedUpdate()
    {
        if (_isSpin)
        {
            Spin();
        }
    }

    private void SpinInput(InputAction.CallbackContext context)
    {
        // スピン中の再入力は受け付けない
        if (_isSpin)
        {
            return;
        }

        // 1回目の入力、または受付時間が終了している場合
        if (!_doubleTapTimer.IsRunning)
        {
            _doubleTapTimer.Start(_spinInputWindow);
            return;
        }

        // 2回目の入力
        _doubleTapTimer.Cancel();

        SpinEnter();
    }

    private void SpinEnter()
    {
        _isSpin = true;
        _spinElapsedTime = 0f;

        // 開始時の回転を保存
        _firstRotation = _myRigidbody.rotation;
    }

    private void Spin()
    {
        _spinElapsedTime += Time.fixedDeltaTime;

        float duration = Mathf.Max(_spinDuration, 0.001f);

        // 0～1の進行度
        float progress = Mathf.Clamp01(_spinElapsedTime / duration);

        Quaternion spinRotation =
            _firstRotation * Quaternion.Euler(0f, 360f * progress, 0f);

        _myRigidbody.MoveRotation(spinRotation);

        if (progress >= 1f)
        {
            _myRigidbody.rotation = _firstRotation;
            _isSpin = false;
        }
    }
}
