using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;

public class CameraTargetMove : MonoBehaviour
{
    [SerializeField] private Rigidbody _playerRigidBody = default;
    [SerializeField] private float _cameraDistance = 3.0f;
    [SerializeField] private PlayerInput _playerInput = default;

    private InputData _inputData =default;
    private void Intialize(InputData inputData)
    {
        _inputData = inputData;
    }
    private void Update()
    {
        Vector3 carForceDirection = _playerRigidBody.linearVelocity.normalized;

        transform.rotation = Quaternion.LookRotation(carForceDirection);

        transform.position = _playerRigidBody.transform.position;
    }
    private void ResetRotation()
    {
        Quaternion moveDirection = _playerRigidBody.transform.rotation;

        transform.rotation = moveDirection;
    }
}
