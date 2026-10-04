using UnityEngine;

public class JumpRotation
{
    private CarMove _carMove = default;
    private GroundDetector _groundDetector = default;
    public JumpRotation(CarMove carMove , GroundDetector groundDetector)
    {
        _carMove = carMove;
        _groundDetector = groundDetector;
    }

    public void AirRotateCar()
    {
        Rigidbody rigidbody = _carMove.GetRigidbody;

        float zrotation = rigidbody.angularVelocity.z;


    }
}
