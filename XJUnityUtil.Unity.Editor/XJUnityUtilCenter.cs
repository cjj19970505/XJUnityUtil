using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XJUnityUtil.Unity
{
    public class XJUnityUtilCenter : MonoBehaviour
    {
        public ICommToApplication CommToUnity { get; private set; }

        public EditorCommToApp EditorCommToApp;
        // Start is called before the first frame update
        void Start()
        {
            CommToUnity = EditorCommToApp;
        }

        // Update is called once per frame
        void Update()
        {
            
        }


    }
}

