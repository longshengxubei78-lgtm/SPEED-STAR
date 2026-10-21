using UnityEngine;
using UnityEngine.InputSystem;

public class CameraTargetMove : MonoBehaviour
{
    [SerializeField] private Rigidbody _playerRigidBody = default;
    [SerializeField] private float _cameraDistance = 3.0f;
    [SerializeField] private PlayerInput _playerInput = default;
    private void Update()
    {
        Vector3 carForceDirection = _playerRigidBody.linearVelocity.normalized;

        transform.rotation = Quaternion.Euler(carForceDirection);
    }
    private void ResetRotation()
    {
        Quaternion moveDirection = _playerRigidBody.transform.rotation;

        transform.rotation = moveDirection;
    }
}
