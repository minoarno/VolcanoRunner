using System;
using UnityEngine;

namespace Core
{
    public class GameController : MonoBehaviour
    {
        public static GameController instance;
        
        // Start is called before the first frame update
        void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
