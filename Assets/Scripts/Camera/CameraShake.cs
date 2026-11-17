using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [SerializeField] private CinemachineBasicMultiChannelPerlin _shake = default;
    [SerializeField] private float _amplitude = 1f;
    [SerializeField] private float _frequency = 1f;


    [SerializeField] private LayerMask _wallLayer;

    private Coroutine _shakeCoroutine;
    private void OnCollisionEnter(Collision collision)
    {
        if (((1 << collision.gameObject.layer) & _wallLayer.value) != 0)
        {
            StartCameraShake(_amplitude, _frequency);
        }
    }
    public void StartCameraShake(float amplitude, float frequency)
    {
        if (_shakeCoroutine != null)
        {
            StopCoroutine(_shakeCoroutine);
        }
        _shakeCoroutine = StartCoroutine(ShakeCamera(amplitude, frequency));
    }
    public void SetShake(float amplitude, float frequency)
    {
        if (_shakeCoroutine != null)
        {
            StopCoroutine(_shakeCoroutine);
            _shakeCoroutine = null;
        }
        _shake.AmplitudeGain = amplitude;
        _shake.FrequencyGain = frequency;
    }

    public void StopShake()
    {
        if (_shakeCoroutine != null)
        {
            StopCoroutine(_shakeCoroutine);
            _shakeCoroutine = null;
        }
        _shake.AmplitudeGain = 0f;
        _shake.FrequencyGain = 0f;
    }
    private IEnumerator ShakeCamera(float amplitude, float frequency)
    {
        _shake.AmplitudeGain = amplitude;
        _shake.FrequencyGain = frequency;
        float time = 0f;
        if (amplitude >= frequency)
        {
            time = amplitude;
        }
        else
        {
            time = frequency;
        }
        while(time > 0)
        {
            _shake.AmplitudeGain = time;
            _shake.FrequencyGain = time;
            time -= Time.deltaTime;
            yield return null;
        }
        _shake.AmplitudeGain = 0f;
        _shake.FrequencyGain = 0f;
    }
}
