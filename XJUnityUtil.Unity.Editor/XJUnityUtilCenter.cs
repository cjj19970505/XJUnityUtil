using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XJUnityUtil.Unity
{
    public class XJUnityUtilCenter : MonoBehaviour
    {
        public ICommToApplication CommToApplication { get; private set; }
        public EditorCommToUnity EditorCommToUnity;
        // Start is called before the first frame update
        void Start()
        {
            CommToApplication = EditorCommToUnity;
        }

        // Update is called once per frame
        void Update()
        {
            
        }
    }
}

