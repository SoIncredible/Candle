namespace StateMachine.PlayerState
{
    public class Idle : State<PlayerStateEnum, PlayerController>
    {
        // TODO Eddie 将Animator注册进来
        public Idle(PlayerController controller) : base(controller)
        {
        }

        protected override void OnEnter()
        {
        }

        protected override void OnTick()
        {
        }

        protected override void OnExit()
        {
        }
    }
}