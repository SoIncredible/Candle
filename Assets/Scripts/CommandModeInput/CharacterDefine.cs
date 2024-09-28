using CommandModeInput.CharacterProperties;
using UnityEngine;

namespace CommandModeInput
{
    [CreateAssetMenu(fileName = "CharacterDefine", menuName = "CreateCharacterDefine", order = 0)]
    public class CharacterDefine : ScriptableObject
    {
        // TODO 用来存储Player和AIPlayer的Prefab引用
        // TODO Eddie 将Player和AIPlayer统一继承自Character基类
        public GameObject PlayerPrefab;
        // public GameObject AIPlayerPrefab;
        
        // 输入映射
        public InputMapping.InputMapping PlayerInputMapping;
        // public InputMapping.InputMapping AIPlayerInputMapping;

        // Entity的属性 速度、生命值等
        public PlayerEntityProperty PlayerEntityProperty;
        // public AIPlayerEntityProperty AIPlayerEntityProperty;
    }
}