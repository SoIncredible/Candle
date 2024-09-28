using System.Collections.Generic;
using CommandModeInput.Entity;
using CommandModeInput.Utils;
using UnityEngine;

namespace CommandModeInput.Manager
{
    public enum PlayerType
    {
        None,
        Player,
        AIPlayer,
    }
    public class CharacterManager : MonoBehaviour
    {
        [Tooltip("存储了所有的Player需要的信息：Player的属性、Prefab、InputMap等")]
        [SerializeField] private CharacterDefine charactersInfo;

        private PlayerPool _playerPool;
        
        private List<CharacterEntityBase> _players = new List<CharacterEntityBase>();

        //--------------------------------------------------------------------------------
        // 主要生命周期
        //--------------------------------------------------------------------------------
        private void Awake()
        {
            
        }

        private void Start()
        {
            // 初始化缓存池
            InitPlayerPool();
            
            InitPlayers();
        }

        private void InitPlayers()
        {
            // TODO AI逻辑
            _players = new List<CharacterEntityBase>(4) { _playerPool.CreatePlayer(PlayerType.Player) };
        }

        private void Update()
        {
            foreach (var player in _players)
            {
                player.OnUpdate();
            }
        }

        private void FixedUpdate()
        { 
            
            foreach (var player in _players)
            {
                player.OnFixedUpdate();
            }
        }
        

        //--------------------------------------------------------------------------------
        // 缓存池相关
        //--------------------------------------------------------------------------------
      
        private void InitPlayerPool()
        {
            _playerPool = new PlayerPool();
            _playerPool.Init(charactersInfo);
        }

        private void ReleasePlayerPool()
        {
            _playerPool.Release();
            _playerPool = null;
        }
    }
}