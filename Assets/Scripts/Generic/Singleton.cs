using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Generic
{
    public class Singleton<T> : MonoBehaviour where T : Component
    {
        private static T instance;

        public static T Instance { get => instance; private set => instance = value; }

        protected virtual void Awake()
        {
            if (Instance != null)
            {
                DestroyImmediate(gameObject);
                return;
            }
            
            Instance = this.GetComponent<T>();
            DontDestroyOnLoad(gameObject);
        }
    }
}
