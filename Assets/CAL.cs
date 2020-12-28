using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Diagnostics;//包含的debug与unity冲突
using System;

public class CAL : MonoBehaviour
{
    public float Speed;//速度
    public float P_Distance;//单位距离
    bool[,] Sensor;//传感器布局
    bool[] SensorFlag = new bool[96];//传感器遍历
    float CurDis;//当前两传感器间距
    public int Mod;//目标模式：0时间优先1距离优先
   
   void Start()//初始化button
    {
        
        Button btn = this.GetComponent<Button>();
        btn.onClick.AddListener(OnClick);
    }

    public void Click()
    {
        string answer = "", cur_answer_x = "", cur_answer_y = "";
        float time = 0, T_dis = 0;
        Sensor = GameObject.Find("Canvas/Sensor").GetComponent<LOG>().Sensor;
        //获取传感器控制器

        Speed = float.Parse(GameObject.Find("Canvas/IO/InputSpeed/Text").GetComponent<Text>().text);
        P_Distance = float.Parse(GameObject.Find("Canvas/IO/InputDistance/Text").GetComponent<Text>().text);
        //获取速度和单位距离

        int StartPoint_x = int.Parse(GameObject.Find("Canvas/IO/InputX/Text").GetComponent<Text>().text) - 1;
        char y_ = char.Parse(GameObject.Find("Canvas/IO/InputY/Text").GetComponent<Text>().text);
        //获取起点坐标

        Mod = GameObject.Find("Canvas/IO/Dropdown").GetComponent<Dropdown>().value;
        //目标模式

        int StartPoint_y = 0;
        switch (y_)
        {
            case 'A': StartPoint_y = 0; break;
            case 'B': StartPoint_y = 1; break;
            case 'C': StartPoint_y = 2; break;
            case 'D': StartPoint_y = 3; break;
            case 'E': StartPoint_y = 4; break;
            case 'F': StartPoint_y = 5; break;
            case 'G': StartPoint_y = 6; break;
            case 'H': StartPoint_y = 7; break;
        }
        // UnityEngine.Debug.Log(StartPoint_x + StartPoint_y);

        for (int i = 0, k = 0; i < 8; i++)
        {
            for (int j = 0; j < 12; j++, k++)
            {
                Sensor[i, j] = GameObject.Find("Canvas/Sensor").GetComponent<LOG>().toggles[k].isOn;
                SensorFlag[k] = Sensor[i, j];
                //  UnityEngine.Debug.Log(" i:" + i + " j:" + j + " is:" + Sensor[i, j]);
            }
        }
        //刷新传感器布局
        SensorFlag[StartPoint_y * 12 + StartPoint_x] = false;
        //初始化遍历表
        cur_answer_y = string.Format("{0:D1}", StartPoint_x + 1);
        // cur_answer_y = string.Format("{0:'ABC'}", cur_answer_y);
        answer = string.Concat(answer, y_, cur_answer_y);
        // UnityEngine.Debug.Log("answer"+ answer);
        //初始化起点

        ////////////////////////////
        //初始化结束，开始计算路径//
        ////////////////////////////

        System.Diagnostics.Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        //开始监视代码运行时间

        int cur = PathFind(StartPoint_y, StartPoint_x);
        //获取第一个答案节点
        T_dis += CurDis;
        //记入总路程

        while (cur != -1)//若当前节点未路过
        {

            int cur_x = cur / 12, cur_y = cur % 12;

            switch (cur_x)
            {
                case 0: cur_answer_y = "A"; break;
                case 1: cur_answer_y = "B"; break;
                case 2: cur_answer_y = "C"; break;
                case 3: cur_answer_y = "D"; break;
                case 4: cur_answer_y = "E"; break;
                case 5: cur_answer_y = "F"; break;
                case 6: cur_answer_y = "G"; break;
                case 7: cur_answer_y = "H"; break;
            }
            cur_answer_x = string.Format("{0:D1}", cur_y + 1);
            // cur_answer_y = string.Format("{0:'ABC'}", cur_answer_y);
            answer = string.Concat(answer, " => ", cur_answer_y, cur_answer_x);
            //UnityEngine.Debug.Log("answer" + answer);
            //输出当前节点

            cur = PathFind(cur_x, cur_y);
            //寻找下一节点
            T_dis += CurDis;
            //记入总路程
        }
        if (cur == -1)//当前节点已遍历&无剩余节点
        {

            GameObject.Find("Canvas/Text").GetComponent<Text>().text = answer;
            //输出路径结果
        }
        //运行完毕

        stopwatch.Stop();
        //  停止监视
        TimeSpan timespan = stopwatch.Elapsed;
        //  获取当前实例测量得出的总时间

        double milliseconds = timespan.TotalMilliseconds;
        //  总毫秒数

        string answer_time = "";
        time = (P_Distance * T_dis) / Speed;
        answer_time = string.Format("{0:#.##}", time);
        if (Mod == 0)
        {
            answer_time = string.Concat("时间最优移动用时： ", answer_time, "\n程序运行耗时：", milliseconds, "ms");
        }
        else
        {
            answer_time = string.Concat("距离最优移动距离： ", P_Distance * T_dis, "\n程序运行耗时：", milliseconds, "ms");
        }
        GameObject.Find("Canvas/IO/Text").GetComponent<Text>().text = answer_time;
        //输出程序运行结果与耗时
        stopwatch.Reset();
        //计时器复位
    }

    private void OnClick()//button点击时触发
    {
       
    }

    private int PathFind(int cur_x, int cur_y)//贪心寻路
    {
        float[] length=new float[96];
        //储存当前节点到每个传感器的距离
        int cur=-1,flag = 0;
        
        for (int i = 0,k=0; i < 8; i++)
        {
            for (int j = 0; j < 12; j++,k++)
            {
                if (Sensor[i, j]&& SensorFlag[k])
                {
                    if (flag == 0)
                    {
                        cur = k;
                        flag = 1;
                        //找到第一个开启的传感器
                    }
                   // UnityEngine.Debug.Log("cur_x"+cur_x+ "cur_y" + cur_y+"i"+i+"j"+j);
                    length[k]= GetDistance(cur_x, cur_y, i,j);
                    //用数组记录该节点到各点的距离
                    if (length[cur] > length[k])
                    {
                        cur = k;
                        SensorFlag[k] = false;
                        //  UnityEngine.Debug.Log("cur" + cur);
                        //距离更近，更新结果
                    }
                   // UnityEngine.Debug.Log("k" + k+ "length[k]" + length[k]);
                }
                else
                {
                    length[k] = 0;
                }
            }
        }
        UnityEngine.Debug.Log("cur:" + cur);
        if(cur!=-1)
        SensorFlag[cur] = false;
        if (cur != -1)
            CurDis = length[cur];
        else CurDis = 0;
        // UnityEngine.Debug.Log("cur:"+cur+" curdis="+CurDis);
        return cur;
        //返回距离最近的传感器的编号
    }

    float GetDistance(int StartPoint_x, int StartPoint_y, int EndPoint_x, int EndPoint_y)//计算距离
    {
        
            int cntX = StartPoint_x - EndPoint_x;
            cntX = cntX < 0 ? -cntX : cntX;
            // UnityEngine.Debug.Log(cntX);
            int cntY = StartPoint_y - EndPoint_y;
            //UnityEngine.Debug.Log(cntY);
            cntY = cntY < 0 ? -cntY : cntY;
           if (Mod == 0)
        { 
            // 判断到底是那个轴相差的距离更远
            if (cntX > cntY)
            {
                // UnityEngine.Debug.Log(cntX);
                return cntX;
            }
            else
            {
                // UnityEngine.Debug.Log(cntY);
                return cntY;
            }
        }//时间优先
        else
        {
            double f;
            float a;
            if (cntX < cntY)
            {
                 
                f = System.Math.Sqrt(cntX* cntX+ cntX * cntX);
                 a = (float)f + (cntY - cntX);
                UnityEngine.Debug.Log(a);
                return a;
            }
            else
            {
                f = System.Math.Sqrt(cntY * cntY + cntY * cntY);
                a = (float)f + (cntX - cntY);
                UnityEngine.Debug.Log(a);
                return a;
            }
             
        }//距离优先
    }
}

