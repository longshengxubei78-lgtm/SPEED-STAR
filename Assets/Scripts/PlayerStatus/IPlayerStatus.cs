using UnityEngine;
using XqLua;

public interface IPlayerStatus
{
    float CurrentSpeed { get; }
    InputInfo _inputInfo { get; }
    IPublisher<CarState, CarState> _onStateChanged { get; }
    Transform _playerTransform { get; }
}
