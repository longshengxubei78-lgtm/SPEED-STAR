using UnityEngine;
public class CarDriver
{
    private Transform _myTransForm = default;
    private CarMove _carMove = default;
    private UphillDeceleration _deceleration;
    private InputData _inputData;

    private float _rotateSpeed=25f;
    private float _moveForce=20;

    float driftMultiplier = 1.5f;
    public CarDriver(CarMove move,Transform myTransform , float moveForce, InputData inputData) 
    {
        _carMove = move;
        _myTransForm = myTransform;
        _moveForce = moveForce;
        _inputData = inputData;

        _deceleration = new UphillDeceleration(_myTransForm);
    }
    public void CarUpdate()
    {
        Vector3 linearVelocity = _carMove.GetRigidbody.linearVelocity;
        float rotateSpeed = _rotateSpeed * driftMultiplier;

        float horizontalValue = _inputData._inputHorizontal;
        float verticalValue = _inputData._inputVertical;

        Debug.Log($"Horizontal: {horizontalValue}, Vertical: {verticalValue}");
        bool isDrifting = _inputData._isDrifting;

        float driftMultiplierValue = 1f;
        if (isDrifting)
        {
            driftMultiplierValue = Drifting(horizontalValue);
            _carMove.Drift();
        }
        ForcePlayerCar(_myTransForm.forward, _moveForce * verticalValue);
        if(verticalValue != 0 || horizontalValue != 0)
        {
            PerspectiveChanges(rotateSpeed, horizontalValue, driftMultiplierValue);
        }
        SpeedLimit();
        _carMove.Tick();
    }
    public void SpeedLimit()
    {
        Rigidbody carRidibody = _carMove.GetRigidbody;

        _carMove.MovementSpeedLimit(carRidibody.linearVelocity);
    }
    private void ForcePlayerCar(Vector3 CarFoward, float MoveForce)
    {
        float decelertion = _deceleration.CalculateDecelerationRate();
        float force = decelertion * MoveForce;

        Vector3 MoveFoward = CarFoward;

        MoveFoward = MoveFoward.normalized;
        _carMove.MoveForceCar(MoveFoward, force);
    }
    private float Drifting(float horizontal)
    {
        float driftMultiplier = horizontal * 2;
        driftMultiplier = Mathf.Abs(driftMultiplier);
        return driftMultiplier;
    }
    private void PerspectiveChanges(float rotateSpeed, float horizontal,float driftMultiplier)
    {
        float angle = horizontal * rotateSpeed * Time.fixedDeltaTime * driftMultiplier;

        Quaternion rotation =
            Quaternion.Euler(0f, angle, 0f);
        _carMove.TurnCar(rotation);
    }
    public float GetCurrentSpeed()
    {
        Rigidbody carRidibody = _carMove.GetRigidbody;
        float currentSpeed = carRidibody.linearVelocity.magnitude;
        return currentSpeed;
    }
}
