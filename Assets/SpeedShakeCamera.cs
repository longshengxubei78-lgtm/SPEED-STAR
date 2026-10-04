using UnityEngine;

public class SpeedShakeCamera : MonoBehaviour
{
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private CameraShake _cameraShake;

    private float _minCameraShakeValue = 0.0f;
    private float _maxCameraShakeValue = 0.4f;
    [SerializeField] private float _speedThreshold = 10f;
    [SerializeField] private float _maxSpeedForFullShake = 20f;

    private void Update()
    {
        float currentSpeed = _rb.linearVelocity.magnitude;

        if (currentSpeed > _speedThreshold)
        {
            float shakevalue = Mathf.InverseLerp(_speedThreshold, _maxSpeedForFullShake, currentSpeed);
            float value = Mathf.Lerp(_minCameraShakeValue, _maxCameraShakeValue, shakevalue);
            _cameraShake.SetShake(value, value);
        }
        else
        {
            _cameraShake.StopShake();
        }
    }
}