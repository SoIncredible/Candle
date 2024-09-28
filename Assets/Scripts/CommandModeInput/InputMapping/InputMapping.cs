using System;
using CommandModeInput.InputModule.InputForm;
using UnityEngine;

namespace CommandModeInput.InputMapping
{
    public enum InputActionType
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

    [Serializable]
    public struct InputDataInfo
    {
        // Form制定这个输入是一个按键类型还是逻辑类型
        [Tooltip("定义该行为的触发类型是什么？按下某个按钮？还是由逻辑控制")]
        public InputFormType InputFormType;
        [Tooltip("定义该行为会造成什么表现？前进？跳跃？")]
        public CommandType CommandType;
        
        // TODO Eddie 为InputDataInfo写一个Custom的Inspector 当选择InputFormType是Logic的时候不展示KeyCode
        [Tooltip("如果该行为的触发类型是按键，那么是按下哪个键的时候触发呢？")]
        public KeyCode KeyCode;
    }

    // 建立按键和行为的映射
    [CreateAssetMenu(fileName =  "InputMap",menuName = "InputMapData", order = 0)]
    public class InputMapping : ScriptableObject
    {
        public InputDataInfo[] InputDataInfos;
    }
}