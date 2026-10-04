using UnityEngine;
using XqLua;

public class TestReactive : MonoBehaviour
{
    [SerializeField]private TestXqLua lua ;
    int aa;
    private void OnEnable()
    {
        lua = GetComponent<TestXqLua>();
        lua.OnDamage.Subscribe(TestInvoke);
    }
    void Update()
    {
        print(lua);
    }
    private void TestInvoke(int bb)
    {
        print(bb);
        
    }
}
