using UnityEngine.InputSystem;
public class InputReader
{
    private readonly InputAction _horizontal, _vertical, _drift;

    public InputReader(PlayerInput playerInput)
    {
        _horizontal = playerInput.actions["Horizontal"];
        _vertical = playerInput.actions["Vertical"];
        _drift = playerInput.actions["Drift"];
    }

    public InputInfo GetInputInfo()
    {
        float horizontal = _horizontal.ReadValue<float>();
        float vertical = _vertical.ReadValue<float>();
        bool isDrifted = _drift.ReadValue<float>() > 0.5f;
        return new InputInfo(horizontal, vertical, isDrifted);
    }
}