using UnityEngine;
public class CameraTargetMove : MonoBehaviour
{
    [SerializeField] private Rigidbody _playerRigidBody = default;
    [SerializeField] private CarController _carController = default;
    [SerializeField] private float _rotateSpeed = 10.0f;
    private void LateUpdate()
    {
        switch (_carController._currentState)
        {
            case CarState.Idle:
                RotateTo(transform.rotation);
                break;
            case CarState.Move:
                MoveRotation();
                break;
            case CarState.Drift:
                MoveRotation();

                break;
        }
    }
    private void DriftCamera()
    {

    }
    private void MoveRotation()
    {
        Vector3 velocity = _playerRigidBody.linearVelocity;
        if (velocity.sqrMagnitude < 0.01f)
        {
            return;
        }

        RotateTo(Quaternion.LookRotation(velocity.normalized));
    }
    private void RotateTo(Quaternion target)
    {
        transform.rotation = Quaternion.Slerp(
            transform.rotation, target, _rotateSpeed * Time.deltaTime);
    }
}
