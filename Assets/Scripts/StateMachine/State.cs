namespace StateMachine
{
    public class State<TState, THost>
    {
        public TState CurrentState;
        public bool HasTargetState;
        public TState TargetState;

        private THost _host;

        protected State(THost host)
        {
            _host = host;
        }
      
        public void Enter()
        {
            HasTargetState = false;
            OnEnter();
        }

        public void Exit()
        {
            OnExit();
        }
        
        public void Tick()
        {
            OnTick();
        }
        
        protected virtual void OnEnter()
        {
        }
        
        protected virtual void OnTick()
        {
        }

        protected virtual void OnExit()
        {
        }

        public void ChangeState(TState state)
        {
            HasTargetState = true;
            TargetState = state;
        }
    }
}