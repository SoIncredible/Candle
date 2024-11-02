using Handler;
using StateMachine;
using StateMachine.PlayerState;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Camera camera;
    private AnimatorHandler _animatorHandler;
    private PlayerMovementHandler _playerMovementHandler;
    
    private StateMachine<PlayerStateEnum, State<PlayerStateEnum, PlayerController>, PlayerController> _stateMachine;

    private void Awake()
    {
        Init();
    }

    // TODO Eddie 上层控制
    public void Init()
    {
        if (camera == null && Camera.main == null)
        {
            Debug.LogError("[PlayerController] No camera or default camera");
            return;
        }
        
        _animatorHandler = new AnimatorHandler();
        _playerMovementHandler = new PlayerMovementHandler(camera, GetComponent<Rigidbody>(),transform);
        InitStateMachine();
    }

    // TODO Eddie 上层控制
    public void Release()
    {
        ReleaseStateMachine();
    }

    // 状态机生命周期
    private void InitStateMachine()
    {
        _stateMachine = new StateMachine<PlayerStateEnum, State<PlayerStateEnum, PlayerController>, PlayerController>();
        
        _stateMachine.AddState(PlayerStateEnum.Idle, new Idle(this));
        _stateMachine.AddState(PlayerStateEnum.Move, new Move(this));
        
        _stateMachine.ChangeState(PlayerStateEnum.Idle);
    }

    private void ReleaseStateMachine()
    {
    }
    
    private void Update()
    {
        _stateMachine?.Tick();
        _playerMovementHandler?.Tick();
        _animatorHandler?.Tick();
    }
}