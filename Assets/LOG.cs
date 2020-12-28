using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LOG : MonoBehaviour
{
    
    public Toggle[] toggles;
    public bool[,] Sensor = new bool[8, 12];
    // Start is called before the first frame update
    void Start()
    {
        Screen.SetResolution(1280,720,false/*是否为窗口*/);
        //固定画面分辨率

       
        int i, j,n=0;

        for (i = 0; i < 8; i++)
        {
            for (j = 0; j < 12; j++)
            {
                Sensor[i,j] = toggles[n].isOn;
                n++;
               // Debug.Log(" i:"+i+" j:"+j +" is:"+Sensor[i, j]);
            }
        }
     //储存传感器状态表
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
