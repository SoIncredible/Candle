using CommandModeInput.Entity;
using UnityEngine;

namespace New.EntityComponent
{
    public class PhysicsComponent : EntityComponentBase
    {
        private readonly Rigidbody _rigidBody;
        
        public PhysicsComponent(Rigidbody rigidBody,EntityBase entityBase) : base(entityBase)
        {
            _rigidBody = rigidBody;
        }

        public override void Receive()
        {
           
        }

        public override void OnFixedUpdate()
        {
            PerformMovement();
            PerformRotate();
        }

        private void PerformMovement()
        {
            Vector3 movement = CameraDirection(((PlayerEntity)EntityBase).moveDir) * ((PlayerEntity)EntityBase)._speed * Time.deltaTime;
            _rigidBody.MovePosition(((PlayerEntity)EntityBase).transform.position + movement);
        }

        private void PerformRotate()
        {
            if (((PlayerEntity)EntityBase).moveDir.magnitude <= 0.01f) return;
            var rotation = Quaternion.Slerp(_rigidBody.rotation,
                Quaternion.LookRotation (CameraDirection(((PlayerEntity)EntityBase).moveDir)),
                ((PlayerEntity)EntityBase).turnSpeed);
            
            _rigidBody.MoveRotation(rotation);
        }
        
        private Vector3 CameraDirection(Vector3 movementDirection)
        {
            var transform1 = ((PlayerEntity)EntityBase).cameraParentTrans.transform;
            var cameraForward = transform1.forward;
            var cameraRight = transform1.right;

            cameraForward.y = 0f;
            cameraRight.y = 0f;
        
            return cameraForward * movementDirection.z + cameraRight * movementDirection.x; 
        }
    }
}