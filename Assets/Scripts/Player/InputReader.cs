using UnityEngine.InputSystem;
using XqLua;

public class InputReader
{
    private PlayerInput _inputActions = default;

    private InputPresenter _inputPresenter = default;

    public InputReader(InputPresenter inputPresenter, PlayerInput playerInput)
    {
        _inputPresenter = inputPresenter;

        _inputActions = playerInput;

        _inputActions.actions["Drift"].performed += _inputPresenter.SetDriftValue;
        _inputActions.actions["Drift"].canceled += _inputPresenter.SetDriftValue;
    }
    public void Dispose()
    {
        _inputActions.actions["Drift"].performed -= _inputPresenter.SetDriftValue;
        _inputActions.actions["Drift"].canceled -= _inputPresenter.SetDriftValue;
    }
    public void Tick()
    {
        float inputHorizontal = _inputActions.actions["Horizontal"].ReadValue<float>();
        float inputVertical = _inputActions.actions["Vertical"].ReadValue<float>();

        _inputPresenter.SetMoveValue(inputHorizontal, inputVertical);
    }
}