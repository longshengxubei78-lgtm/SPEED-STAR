using UnityEngine;
using XqLua;

public interface IPlayerData
{
    float CurrentSpeed { get; }
    InputInfo _inputInfo { get; }
    IPublisher<CarState, CarState> _onStateChanged { get; }
    Transform _playerTransform { get; }
}
