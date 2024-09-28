using System.Collections.Generic;
using CommandModeInput.CharacterProperties;
using CommandModeInput.Entity;
using CommandModeInput.Manager;
using New.EntityComponent;
using UnityEngine;

namespace CommandModeInput.Utils
{
    // Player缓存池
    public class PlayerPool
    {
        private CharacterDefine _characterPrefab;
        
        private PlayerEntityProperty _playerProperty;
        private AIPlayerEntityProperty _aiPlayerProperty;

        private InputMapping.InputMapping _playerMapping;
        private InputMapping.InputMapping _aiPlayerMapping;
        
        // 是否已经初始化了玩家操控的Player
        private bool _hasInitPlayer;
        
        private Dictionary<PlayerType, Stack<CharacterEntityBase>> _playerCacheDic;
        
        public void Init(CharacterDefine characterDefine)
        {
            _playerCacheDic = new Dictionary<PlayerType, Stack<CharacterEntityBase>>(4);
            
            _characterPrefab = characterDefine;
            _playerProperty = characterDefine.PlayerEntityProperty;
            // _aiPlayerProperty = characterDefine.AIPlayerEntityProperty;

            _playerMapping = characterDefine.PlayerInputMapping;
            // _aiPlayerMapping = characterDefine.AIPlayerInputMapping;
            
            _hasInitPlayer = false;
        }

        public void Release()
        {
            _playerCacheDic.Clear();
            _playerProperty = null;
        }
        
        public CharacterEntityBase CreatePlayer(PlayerType playerType)
        {
            if (playerType == PlayerType.Player && _hasInitPlayer)
            {
                Debug.LogWarning("Character controlled by Player has been instantiate!!");
                return null;
            }
            
            if (!_playerCacheDic.ContainsKey(playerType))
            {
                _playerCacheDic.Add(playerType, new Stack<CharacterEntityBase>());
            }

            var cacheStack = _playerCacheDic[playerType];

            // 从缓存池中拿Player
            switch (playerType)
            {
                case PlayerType.Player:
                    PlayerEntity player;
                    if (cacheStack.Count > 0)
                    {
                        player = cacheStack.Pop() as PlayerEntity;
                    }
                    else
                    {
                        player = Object.Instantiate(_characterPrefab.PlayerPrefab).GetComponent<PlayerEntity>();
                        var inputComponent = new PlayerInputComponent(_playerMapping, player);
                        
                        var playerAnim = player.GetComponent<Animator>();
                        var animatorComponent = new AnimatorComponent(playerAnim, player);

                        var rigidBody = player.GetComponent<Rigidbody>();
                        var physicsComponent = new PhysicsComponent(rigidBody, player);
                        
                        EntityComponentBase[] componentBases = { inputComponent, animatorComponent, physicsComponent };
                        player.OnCreate(componentBases);
                    }

                    if (player != null)
                    {
                        player.Init(_playerProperty, PlayerType.Player);

                        if (_hasInitPlayer)
                        {
                            Debug.LogWarning("Character controlled by Player has been instantiate!!");
                        }

                        _hasInitPlayer = true;

                        return player;
                    }
                    
                    Debug.LogError("Wrong Player Type!!");
                    return null;

                // case PlayerType.AIPlayer:
                //     AIPlayerEntity aiPlayer;
                //     if (cacheStack.Count > 0)
                //     {
                //         aiPlayer = cacheStack.Pop() as AIPlayerEntity;
                //     }
                //     else
                //     {
                //         aiPlayer = Object.Instantiate(_characterPrefab.AIPlayerPrefab).GetComponent<AIPlayerEntity>();
                //         aiPlayer.OnCreate();
                //     }
                //
                //     if (aiPlayer != null)
                //     {
                //         aiPlayer.Init(_aiPlayerProperty, PlayerType.AIPlayer,_aiPlayerMapping);
                //         return aiPlayer;
                //     }
                //     
                //     Debug.LogError("Wrong Player Type!!");
                //     return null;
            }
            
            Debug.LogError("Wrong Player Type!!");
            return null;
        }

        public void DestroyPlayer(CharacterEntityBase player)
        {
            if (player.playerType == PlayerType.Player)
            {
                _hasInitPlayer = false;
            }
            var stack = _playerCacheDic[player.playerType];
            stack.Push(player);

        }
    }
}