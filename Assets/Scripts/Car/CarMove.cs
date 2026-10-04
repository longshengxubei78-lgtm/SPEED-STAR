using UnityEngine;
using UnityEngine.UIElements;
public class CarMove
{
    private Rigidbody _myRigibody = default;

    private float _sideDecelerationRate = 0.9f;

    private float _maxSpeed = 0.0f;
    public Rigidbody GetRigidbody
    {
        get { return _myRigibody; }
    }
    public CarMove(Rigidbody rb, float maxSpeed)
    {
        _myRigibody = rb;
        _maxSpeed = maxSpeed;
    }
    public void Tick()
    {
        ReduceSidewaysVelocity();
    }
    public void MovementSpeedLimit(Vector3 getVelocity)
    {
        _myRigibody.linearVelocity = Vector3.ClampMagnitude(getVelocity, _maxSpeed);
    }
    public void MoveForceCar(Vector3 force, float _moveForce)
    {
        ForceLerp(ref _moveForce);
        _myRigibody.AddForce(force * _moveForce);
    }
    private void ForceLerp (ref float force)
    {
        float currentSpeed = _myRigibody.linearVelocity.magnitude;

        currentSpeed = currentSpeed / _maxSpeed;

        if(currentSpeed > 0.5f)
        {
            currentSpeed = Mathf.Abs(currentSpeed - 1);

            force *= currentSpeed;
        }
    }
    private void ReduceSidewaysVelocity()
    {
        Vector3 currentVelocity = _myRigibody.linearVelocity;

        Vector3 carForward = _myRigibody.transform.forward;
        Vector3 carRight = _myRigibody.transform.right;

        float forwardSpeed = Vector3.Dot(currentVelocity, carForward);
        float sidewaysSpeed = Vector3.Dot(currentVelocity, carRight);

        sidewaysSpeed *= _sideDecelerationRate;

        Vector3 newVelocity =
            carForward * forwardSpeed +
            carRight * sidewaysSpeed;

        newVelocity.y = currentVelocity.y;

        _myRigibody.linearVelocity = newVelocity;
    }
    public void TurnCar(Quaternion rotation)
    {
        _myRigibody.MoveRotation(_myRigibody.rotation * rotation);
    }
}