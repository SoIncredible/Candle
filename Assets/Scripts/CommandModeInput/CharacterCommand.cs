
using System;

namespace CommandModeInput
{
    /// <summary>
    /// 指明这个Command的作用是什么
    /// </summary>
    public enum CommandType
    {
        None = 0,
        MoveForward = 1,
        MoveBackward = 2,
        MoveLeft = 3,
        MoveRight = 4,
        Jump = 5 ,
        Crunch = 6,
        Attack = 7,
        Length = 8,
    }
    
    public class CharacterCommand
    {
        public CommandType CommandType;
        public Action CommandMarkAction;
    }
}