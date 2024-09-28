using UnityEngine;

namespace CommandModeInput.InputModule.InputForm
{
    public class KeyCodeInputForm : BaseInputForm
    {
        private KeyCode _activeKeyCode = KeyCode.None;
        
        public override void Init(InputFormType type)
        {
            base.Init(type);
        }

        protected override bool CheckInputFormValid()
        {
            return _activeKeyCode != KeyCode.None;
        }

        protected override bool IsInputActive()
        {
            return Input.GetKey(_activeKeyCode);
        }

        public void RebindKeyCode(KeyCode keyCode)
        {
            _activeKeyCode = keyCode;
        }
    }
}