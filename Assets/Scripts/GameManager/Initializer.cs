using UnityEngine;

public class Initializer : MonoBehaviour
{
    [SerializeField] private CarController _carController = default;

    [SerializeField] private CameraTargetMove _cameraTargetMove = default;
    [SerializeField] private SoundBank _soundBank = default;

    void Start()
    {
        CarSoundPresenter carSoundPresenter = new CarSoundPresenter(_carController, _soundBank);
        _cameraTargetMove.Initialize(_carController);
    }
}
