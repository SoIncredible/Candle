using System;
using UnityEngine;

namespace Human
{
    public class BaseHuman : MonoBehaviour
    {
        private static readonly int IsMoving = Animator.StringToHash("IsMoving");

        // 是否正在移动
        protected bool isMoving = false;
        
        // 移动目标点
        private Vector3 _targetPosition;
        
        public float speed = 1.5f;
        
        private Animator _animator;

        // 描述
        public string desc = "";

        protected void MoveTo(Vector3 pos)
        {
            _targetPosition = pos;
            isMoving = true;
            _animator.SetBool(IsMoving, true);
        }

        public void MoveUpdate()
        {
            if (!isMoving)
            {
                return;
            }  
            
            var pos = transform.position;
            transform.position = Vector3.MoveTowards(pos, _targetPosition, speed * Time.deltaTime);
            transform.LookAt(_targetPosition);
            if (Vector3.Distance(pos, _targetPosition) < 0.05f)
            {
                isMoving = false;
                _animator.SetBool(IsMoving, false);
            }
        }

        protected virtual void Start()
        {
            _animator = GetComponent<Animator>();
        }

        protected virtual void Update()
        {
            MoveUpdate();
        }
    }
}