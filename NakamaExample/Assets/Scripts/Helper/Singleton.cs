using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.NakamaManager
{
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static readonly object _lock = new object();
        private static T _instance;

        public static T Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = FindObjectOfType<T>();
                    }

                    return _instance;
                }
            }
        }

        protected virtual void Awake()
        {
            if (Instance != this)
            {
                Destroy(gameObject);
            }
        }

        protected virtual void OnDestroy()
        {
            if (Instance == this)
            {
                _instance = null;
            }
        }
    }
}
