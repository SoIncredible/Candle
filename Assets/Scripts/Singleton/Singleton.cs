namespace Singletons
{
    public class Singleton<T> where T : class, new()
    {
        protected static T _instance = null;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new T();
                }
                return _instance;
            }
        }
        public void DestroySelf ()
        {
            Destroy();
            _instance = null;
        }

        public virtual void Destroy()
        {
            UnRegisterMsg();
        }

        public Singleton()
        {
            Init();
        }

        public void Startup()
        {

        }

        protected virtual void Init()
        {
            RegisterMsg();
        }
		
        protected virtual void RegisterMsg()
        {
        }

        protected virtual void UnRegisterMsg()
        {
        }
        
        public static void ForceDestroy()
        {
            if (_instance != null)
            {
                Singleton<T> temp = (_instance as Singleton<T>);
                if (temp != null)
                {
                    temp.DestroySelf();
                }
            }
            _instance = null;
        }
    }
}