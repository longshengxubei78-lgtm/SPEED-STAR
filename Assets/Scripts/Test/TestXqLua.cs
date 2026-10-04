using System;
using UnityEngine;
using XqLua;
public class TestXqLua : MonoBehaviour
{
    //public Action<int> action;
    public IPublisher<int> OnDamage => _onDamage;

    private Publisher<int> _onDamage=default;
    private bool _isInvincible =false;

    private IDisposableSubscription _subScription=default;

    int _id=3;
    float time=0;
    private void Awake()
    {
        _onDamage = new Publisher<int>();

        _subScription = OnDamage
            .OnCondition(() => !_isInvincible)
            .Subscribe(value =>
            {
                Debug.Log(value);
            });
    }
    private void OnDestroy()
    {
        _subScription.Dispose();
        _onDamage.Dispose();
    }

    private void Damage(int value)
    {
        _onDamage.Invoke(value);
    }
    private void Update()
    {
        time += Time.deltaTime;

        if (_id < time)
        {
            time = 0;
            //action.Invoke(_id);
            _onDamage.Invoke(_id); 
        }
    }
}
