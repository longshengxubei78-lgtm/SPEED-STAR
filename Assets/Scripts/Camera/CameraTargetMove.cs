using UnityEngine;
public class CameraTargetMove : MonoBehaviour
{
    [SerializeField] private Rigidbody _playerRigidBody = default;
    [SerializeField] private CarController _carController = default;
    [SerializeField] private float _driftCameraSideMoveSpeed = 10.0f;
    [SerializeField] private float _cameraSideLength = 5.0f;
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
                DriftCamera(_carController.InputHorizontal);

                break;
        }
    }
    private void DriftCamera(float driftAmount)
    {
        driftAmount *= _cameraSideLength;

        Vector3 targetPosition = _carController.transform.position + transform.right * driftAmount;

        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * _driftCameraSideMoveSpeed);
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
            transform.rotation, target, _driftCameraSideMoveSpeed * Time.deltaTime);
    }
}
