using System;
using Network;
using UnityEngine;

namespace Human
{
    public class BaseHuman : MonoBehaviour
    {
        private static readonly int IsMoving = Animator.StringToHash("IsMoving");
        private static readonly int IsAttacking = Animator.StringToHash("IsAttacking");

        // 是否正在移动
        protected bool isMoving = false;
        
        // 移动目标点
        private Vector3 _targetPosition;
        
        // 是否正在攻击
        internal bool isAttacking = false;
        internal float attackTime = float.MinValue;
        
        public float speed = 1.5f;
        
        private Animator _animator;

        // 描述
        public string desc = "";

        public void MoveTo(Vector3 pos)
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
            AttackUpdate();
        }

        public void Attack()
        {
            isAttacking = true;
            attackTime = Time.realtimeSinceStartup;
            _animator.SetBool(IsAttacking, true);
        }

        public void AttackUpdate()
        {
            if (!isAttacking) return;
            if (Time.realtimeSinceStartup - attackTime < 1.2f) return; // 设计的一次攻击时间时长是1.2s 得看一下我选的动画长度是多少
            isAttacking = false;
            _animator.SetBool(IsAttacking, false);
        }
    }
}