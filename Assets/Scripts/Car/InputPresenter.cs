using UnityEngine.InputSystem;
using UnityEngine;
public class InputPresenter
{
    private  InputData _inputData;
    public InputPresenter(InputData inputData)
    {
        _inputData = inputData;
    }
    public void SetMoveValue(float inputHorizotal,float inputVertical)
    {
        _inputData.SetMove(inputHorizotal, inputVertical);
    }
    public void SetDriftValue(bool isDrift)
    {
        _inputData.SetDrift(isDrift);
    }
    public void SetDriftValue(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _inputData.SetDrift(true);
            Debug.Log("Drift started");
        }
        else if (context.canceled)
        {
            _inputData.SetDrift(false);
            Debug.Log("Drift canceled");
        }
    }
}
