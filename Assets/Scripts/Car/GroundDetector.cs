using UnityEngine;

public class GroundDetector
{
    private Transform _carTransform;

    private float _maxDistance = 2f;
    private Vector3 _halfExtents = new Vector3(0.5f, 0.01f, 1f);
    public GroundDetector(Transform carTransform)
    {
        _carTransform = carTransform;
    }
    private float _startOffset = 0.6f;

    public bool GroundSearch()
    {
        Vector3 origin = _carTransform.position + _carTransform.up * _startOffset;

        bool isGround = Physics.BoxCast(origin, _halfExtents, -_carTransform.up *0.1f, _carTransform.rotation, _maxDistance + _startOffset);
        Debug.Log("GroundSearch: " + isGround);
        return isGround;
    }
    public float GroundDistance()
    {
        RaycastHit hit;
        bool isGroundHit=Physics.Raycast(_carTransform.position, Vector3.down,out hit, Mathf.Infinity);
        if(!isGroundHit)
        {
            return Mathf.Infinity;
        }
        float distance = hit.distance;

        return distance;
    }
}