using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SpeedAudio : MonoBehaviour
{
    [SerializeField]private SoundBank _soundBank = default;
    [SerializeField] private Rigidbody _rigidbody = default;
    [SerializeField] private AudioSource _audioSource = default;

    [Header("Speed")]
    [SerializeField] private float _maxSpeed = 10f;      // この速度でPitch/Volumeが最大になる

    [Header("Pitch")]
    [SerializeField] private float _minPitch = 0.8f;     // 停止時
    [SerializeField] private float _maxPitch = 1.8f;     // 最高速時
    [SerializeField] private float _pitchSmoothTime = 0.1f;

    [Header("Volume")]
    [SerializeField] private float _minVolume = 0.3f;
    [SerializeField] private float _maxVolume = 1.0f;

    private float _pitchVelocity;

    private void Reset()
    {
        _audioSource = GetComponent<AudioSource>();
        _rigidbody = GetComponentInParent<Rigidbody>();
    }

    private void Awake()
    {
        _audioSource.loop = true;
        _audioSource.playOnAwake = false;
        _audioSource.pitch = _minPitch;
        _audioSource.volume = _minVolume;
        _audioSource.Play();
    }

    private void Update()
    {
        float speed = _rigidbody.linearVelocity.magnitude;
        float t = Mathf.InverseLerp(0f, _maxSpeed, speed);   // 0〜1に正規化

        if(speed < 0f)
        {
            _soundBank.Play("Idle");
            _soundBank.Stop("Accel");
        }
        else
        {
            _soundBank.Play("Accel");
            _soundBank.Stop("Idle");
        }

        float targetPitch = Mathf.Lerp(_minPitch, _maxPitch, t);
        _audioSource.pitch = Mathf.SmoothDamp(_audioSource.pitch, targetPitch, ref _pitchVelocity, _pitchSmoothTime);
        _audioSource.volume = Mathf.Lerp(_minVolume, _maxVolume, t);
    }
    //spped0ならｂらけなし
}