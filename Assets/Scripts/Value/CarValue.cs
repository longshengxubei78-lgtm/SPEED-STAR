using UnityEngine;

public struct InputInfo
{
    public float horizontal;
    public float vertical;
    public bool isDrifted;
    public InputInfo (float horizontal, float vertical, bool isDrifted)
    {
        this.horizontal = horizontal;
        this.vertical = vertical;
        this.isDrifted = isDrifted;
    }
}
public struct GroundInfo
{
    public Vector3 groundNormal;
    public bool isGrounded;
    public GroundInfo (Vector3 groundNormal, bool isGrounded)
    {
        this.groundNormal = groundNormal;
        this.isGrounded = isGrounded;
    }
}
