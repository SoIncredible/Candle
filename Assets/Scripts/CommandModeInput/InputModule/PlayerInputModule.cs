using CommandModeInput.Entity;
using CommandModeInput.InputModule.InputForm;
using New.EntityComponent;
using UnityEngine;

namespace CommandModeInput.InputModule
{
    public class PlayerInputModule : CharacterInputModuleBase
    {
        public PlayerInputModule(InputMapping.InputMapping inputMapping, PlayerInputComponent entity) : base(inputMapping, entity)
        {
        }

        //--------------------------------------------------------------------------------
        // 初始化
        //--------------------------------------------------------------------------------

        public override void InitInputModule()
        {
            var infos = InputMap.InputDataInfos;

            // 获得按键->Action之间的绑定
            // 获得Action->具体的Entity的具体行为绑定
            // 最终得到按键->具体Entity的具体行为的绑定
            foreach (var info in infos)
            {
                var characterCommand = new CharacterCommand()
                {
                    CommandType = info.CommandType,
                };
               
                // 绑定命令和要标记修改的状态
                BindCommandAndBehaviour(characterCommand);
                
                BaseInputForm inputForm = null;
                switch (info.InputFormType)
                {
                    case InputFormType.KeyCode:
                        inputForm = new KeyCodeInputForm();
                        inputForm.Init(InputFormType.KeyCode);
                        ((KeyCodeInputForm)inputForm).RebindKeyCode(info.KeyCode);
                        //绑定Input和命令
                        BindInputAndCommand(inputForm, characterCommand);
                        break;
                    case InputFormType.Logic:
                        // TODO Logic的实体需要再确认一下
                        inputForm = new LogicInputForm();
                        inputForm.Init(InputFormType.Logic);
                        BindInputAndCommand(inputForm, characterCommand);
                        break;
                    case InputFormType.None:
                        Debug.LogError("InputFormType is None!");
                        break;
                }
            }
        }

        public override void ReleaseInputModule()
        {
            
        }

        public override void GatherInput()
        {
            foreach (var pair in InputCommandMappingDic)
            {
                var isActive = pair.Key.CheckInputActive();
                if (isActive)
                {
                    pair.Value.CommandMarkAction.Invoke();
                }
            }
        }
    }
}