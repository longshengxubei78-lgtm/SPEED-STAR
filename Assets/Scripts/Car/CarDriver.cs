using UnityEngine;
public class CarDriver
{
    private Transform _myTransForm = default;
    private CarMove _carMove = default;
    private UphillDeceleration _deceleration;
    private InputReader _inputReader;

    private float _rotateSpeed=25f;
    private float _moveForce=20;

    private float _maxSpeed = 0.0f;
    public CarDriver(CarMove move,Transform myTransform , float maxSpeed, float moveForce, InputReader inputReader) 
    {
        _carMove = move;
        _myTransForm = myTransform;
        _maxSpeed = maxSpeed;
        _moveForce = moveForce;
        _inputReader = inputReader;

        _deceleration = new UphillDeceleration(_myTransForm);
    }
    public void CarUpdate(float driftMultiplier)
    {
        _inputReader.Tick();
        float rotateSpeed = _rotateSpeed * driftMultiplier;

        float horizontalValue = _inputReader._inputhorizontal;
        float verticalValue = _inputReader._inputvertical;

        ForcePlayerCar(_myTransForm.forward, _moveForce * verticalValue);
        if(verticalValue != 0)
        {
            PerspectiveChanges(rotateSpeed, horizontalValue);
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
    private void PerspectiveChanges(float rotateSpeed, float horizontal)
    {
        float angle = horizontal * rotateSpeed * Time.fixedDeltaTime;

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
