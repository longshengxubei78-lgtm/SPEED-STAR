using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using XqLua.Extension;
public class CPUStateMachine : MonoBehaviour
{
    [SerializeField] private Transform _playerTransform = default;
    [SerializeField] private Rigidbody _myRigibody = default;
    [SerializeField] private GameObject _firstTarget = default;
    [SerializeField] private RaceFlowManager _raceFlowManager = default;
    private ICarState _currentState = default;
    private CPUMoveTargetSearch _searchTarget = default;

    [SerializeField] private float _maxSpeed = 0.0f;
    [SerializeField] private float _moveForce = 0.0f;


    [SerializeField] private LayerMask _roadLayer;
    [SerializeField] private LayerMask _wallLayer;
    private Dictionary<System.Type, ICarState> states = default;
    private bool _canControl = default;
    private void ApplyPhase(RacePhase phase,RacePhase nextPhase)
    {
        _canControl = nextPhase == RacePhase.Racing;
    }
    private void Awake()
    {
        InputData inputData = new InputData();
        InputPresenter inputPresenter = new InputPresenter(inputData);
        CarMove carMove = new CarMove(_myRigibody, _maxSpeed, _roadLayer);
        GroundDetector _groundDetector = new GroundDetector(transform,_roadLayer);
        CarDriver _driver = new CarDriver(_myRigibody, transform,  _maxSpeed, _moveForce, inputData,_groundDetector);
        _searchTarget = new CPUMoveTargetSearch(transform, _firstTarget ,inputPresenter, _myRigibody, _playerTransform, _wallLayer);

        states = new Dictionary<System.Type, ICarState>
        {
            { typeof(CPUIdleState), new CPUIdleState(this,inputData) },
            { typeof(CPUMoveState), new CPUMoveState(this, _driver, _groundDetector) },
            { typeof(CPUAirState), new CPUAirState(this) }
        };
        SwitchState(typeof(CPUIdleState));
    }
    private void Start()
    {
        _raceFlowManager._OnPhaseChaged.Subscribe(ApplyPhase);
    }
    private void FixedUpdate()
    {
        if (!_canControl)
            return;
        _searchTarget.FixedTick();
        _currentState.FixedTick();
    }
    public void SwitchState(System.Type newStateType)
    {
        if (_currentState != null)
        {
            _currentState.Exit();
        }

        if (states.TryGetValue(newStateType, out ICarState newState))
        {
            _currentState = newState;

            _currentState.Enter();
        }
        else
        {
            Debug.LogError("ステートが見つかりません");
        }
    }
}
