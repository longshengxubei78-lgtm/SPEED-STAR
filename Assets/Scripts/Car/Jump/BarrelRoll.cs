using UnityEngine;
public class BarrelRoll
{
    private Rigidbody _carRgidbody = default;
    private GroundInfo _groundInfo = default;
    public BarrelRoll(Rigidbody carRgidbody,GroundInfo groundInfo)
    {
        _carRgidbody = carRgidbody;
        _groundInfo = groundInfo;
    }
    public void BarrelRollStart()
    {
        //if (_groundInfo.groundDistance == Mathf.Infinity)
        //{
        //    return;
        //}
        //float landingTime = GetLandingTime(_groundInfo.groundDistance);
        //bool shouldRoll = RollJudgement(landingTime);
        Roll();
    }
    public void FixedTick()
    {
        Roll();
    }
    private void BarrelRollEnd()
    {

    }
    private void Roll()
    {
        _carRgidbody.AddTorque(_carRgidbody.transform.forward * 200000f, ForceMode.Force);
    }
    private float GetLandingTime(float fallHeight)
    {
        float landingTime = Mathf.Sqrt((2 * fallHeight) / Physics.gravity.magnitude);
        return landingTime;
    }
    private bool RollJudgement(float landingTime)
    {
        float rollValue = _carRgidbody.angularVelocity.magnitude;

        if (rollValue < 0.001f)
        {
            return false;
        }

        float oneRotationTime = (2.0f * Mathf.PI) / rollValue;


        if(oneRotationTime < landingTime)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
