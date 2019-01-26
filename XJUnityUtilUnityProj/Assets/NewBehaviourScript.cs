using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XJUnityUtil.Unity;

public class NewBehaviourScript : MonoBehaviour
{
    public XJUnityUtilCenter XJUnityUtilCenter;
    public List<string> Messages;
    // Start is called before the first frame update
    void Start()
    {
        XJUnityUtilCenter.CommToApplication.Received += _OnEditorCommToAppReceived;
    }

    private void _OnEditorCommToAppReceived(object sender, string e)
    {
        Messages.Add(e);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
