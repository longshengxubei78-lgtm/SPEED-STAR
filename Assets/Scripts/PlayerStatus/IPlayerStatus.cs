using UnityEngine;
using XqLua;

public interface IPlayerStatus
{
    float _currentSpeed { get; }
    InputInfo _inputInfo { get; }
    IPublisher<CarState, CarState> _onStateChanged { get; }
    Transform _playerTransform { get; }
}
