using System.Collections;
using Singletons;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public MonoBehaviour Behaviour { get; private set; }

    public void SetBehaviour(MonoBehaviour behaviour)
    {
        
    }

    public void StartCoroutine(IEnumerator routine)
    {
        Behaviour.StartCoroutine(routine);
    }
}