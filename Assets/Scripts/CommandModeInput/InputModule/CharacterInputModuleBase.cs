using System.Collections.Generic;
using CommandModeInput.InputModule.InputForm;
using New.EntityComponent;
using UnityEngine;

namespace CommandModeInput.InputModule
{
    public abstract class CharacterInputModuleBase
    {
        protected readonly Dictionary<BaseInputForm, CharacterCommand> InputCommandMappingDic = new Dictionary<BaseInputForm, CharacterCommand>();
        // SO
        protected readonly InputMapping.InputMapping InputMap;
        
        // 需要知道InputComponent
        private readonly PlayerInputComponent _inputComponent;
        
        protected CharacterInputModuleBase(InputMapping.InputMapping inputMapping, PlayerInputComponent inputComponent)
        {
            InputMap = inputMapping;
            _inputComponent = inputComponent;
        }

        /// <summary>
        /// 读取SO中存储的Module Action的映射关系
        /// </summary>
        public abstract void InitInputModule();

        public abstract void ReleaseInputModule();
        
        public abstract void GatherInput();
        
        /// <summary>
        /// 绑定Command和它要改变的状态,需要在Entity中维护一个Command和要改变状态的Action的Dic
        /// </summary>
        /// <param name="command"></param>
        protected void BindCommandAndBehaviour(CharacterCommand command)
        {
            var commandMarkAction = _inputComponent.GetCommandMarkAction(command.CommandType);
            command.CommandMarkAction = commandMarkAction;
        }

        protected void BindInputAndCommand(BaseInputForm inputForm, CharacterCommand action)
        {
            if (!InputCommandMappingDic.TryAdd(inputForm, action))
            {
                Debug.LogWarning($"InputCommandMappingDic has contains the key {inputForm}");
            }
        }
    }
}