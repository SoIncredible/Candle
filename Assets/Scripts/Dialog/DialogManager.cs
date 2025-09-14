using Network;
using Singletons;

namespace Dialog
{
    public class DialogManager : Singleton<DialogManager>
    {
        protected override void Init()
        {
            NetManager.AddListener("Dialog", OnDialog);
        }
        
        
        /// <summary>
        /// 聊天协议
        /// </summary>
        /// <param name="msg"></param>
        private void OnDialog(string msg)
        {   
        
        }
    }
}