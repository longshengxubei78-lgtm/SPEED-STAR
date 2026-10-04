using UnityEngine.InputSystem;
using XqLua;

public class InputReader
{
    private PlayerInput _inputActions = default;

    private CarController _carController = default;

    public float _inputhorizontal { get; private set; }
    public float _inputvertical { get; private set; }

    public InputReader(CarController carController,PlayerInput playerInput)
    {
        _carController = carController;

        _inputActions = playerInput;

        _inputActions.actions["Drift"].performed += _carController.InputDrift;
        _inputActions.actions["Drift"].canceled += _carController.InputDrift;
    }
    public void Dispose()
    {
        _inputActions.actions["Drift"].performed -= _carController.InputDrift;
        _inputActions.actions["Drift"].canceled -= _carController.InputDrift;
    }
    public void Tick()
    {
        _inputhorizontal = _inputActions.actions["Horizontal"].ReadValue<float>();
        _inputvertical = _inputActions.actions["Vertical"].ReadValue<float>();
    }
}