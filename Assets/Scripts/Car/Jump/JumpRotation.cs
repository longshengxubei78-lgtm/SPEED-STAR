using UnityEngine;

public class JumpRotation
{
    //[SerializeField] private float rotations = 3f;
    //[SerializeField] private float duration = 2f;

    private Rigidbody rb;
    //private Quaternion startRotation;
    //private float elapsed;
    //private bool isSpinning;
    private LayerMask _roadLayer = default;
    public JumpRotation(Rigidbody rigidbody,LayerMask roadLayer)
    {
        rb = rigidbody;
        _roadLayer = roadLayer;
    }

    //public void StartSpin(float seconds)
    //{
    //    duration = Mathf.Max(0.01f, seconds);
    //    startRotation = rb.rotation;
    //    elapsed = 0f;
    //    isSpinning = true;
    //}
    //public void EndSpin(float seconds)
    //{
    //    duration = Mathf.Max(0.01f, seconds);
    //    startRotation = default;
    //    elapsed = 0f;
    //    isSpinning = false;
    //}

    public void FixedTick()
    {
        float maxDistance = Mathf.Infinity;
        RaycastHit roadHit;
        bool isRoadHit = Physics.Raycast(rb.transform.position, rb.linearVelocity.normalized, out roadHit, maxDistance, _roadLayer);

        if (isRoadHit)
        {
            float distance = Vector3.Distance(rb.transform.position, roadHit.point);
            float currentSpeed = rb.linearVelocity.magnitude;

            float timeToTarget = 0f;

            //0除算がないようにする
            if (currentSpeed > 0.001f)
            {
                timeToTarget = distance / currentSpeed;
            }
            else
            {
                timeToTarget = float.PositiveInfinity;
            }


        }
    }
    //private bool BarrelSpinDetector()
    //{

    //}
}
