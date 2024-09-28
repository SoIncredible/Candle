namespace FiniteStateMachine.Core
{
    /// <summary>
    /// 游戏中只有玩家和敌人会使用到状态机 而玩家和敌人的行为差不多 所以用一个StateType进行表示
    /// </summary>
    public enum StateType
    {
        None = 0,
        Idle,
        Move,
        Jump,
        Crouch,
        Attack,
    }

    public enum GlobalParamType
    {
        None = 0,
    }

    public class FiniteStateMachineDefine
    {
    }
}