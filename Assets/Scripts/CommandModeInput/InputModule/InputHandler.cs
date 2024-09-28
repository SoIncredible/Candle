
using UnityEngine;

namespace CommandModeInput.InputModule
{
    public class InputHandler : MonoBehaviour
    {
        [SerializeField] private float mouseX;
        [SerializeField] private float mouseY;

        [SerializeField] public float horizontal;
        [SerializeField] public float vertical;
        
        public float MoveAmount;
        
        private Vector2 _movementInput;
        private Vector2 _cameraInput;
        
        private void OnEnable()
        {
            // TODO Cancel return
            return;
            
           
        }

        private void OnDisable()
        {
            // TODO Cancel return
            return;
         
        }

        public void TickInput(float delta)
        {
            TickMovement(delta);   
        }
        
        private void TickMovement(float delta)
        {
            horizontal = _movementInput.x;
            vertical = _movementInput.y;
            MoveAmount = Mathf.Clamp01(Mathf.Abs(horizontal) + Mathf.Abs(vertical));
            mouseX = _cameraInput.x;
            mouseY = _cameraInput.y;
        }
    }
}