using UnityEngine;

namespace CommandModeInput.InputModule.InputForm
{
    public enum InputFormType
    {
        None,
        KeyCode,
        Logic,
    }
    public abstract class BaseInputForm
    {
        protected InputFormType _inputFormType;

        public virtual void Init(InputFormType inputFormType)
        {
            _inputFormType = inputFormType;
        }

        protected abstract bool CheckInputFormValid();

        public bool CheckInputActive()
        {
            if (CheckInputFormValid())
            {
                return IsInputActive();
            }

            Debug.LogError("Please Check Input Form Validation");
            return false;
        }
        
        protected abstract bool IsInputActive();
    }
}