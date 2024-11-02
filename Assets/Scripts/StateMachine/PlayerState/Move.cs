namespace StateMachine.PlayerState
{
    public class Move : State<PlayerStateEnum, PlayerController>
    {
        // TODO Eddie 将Animator注册进来
        public Move(PlayerController controller) : base(controller)
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