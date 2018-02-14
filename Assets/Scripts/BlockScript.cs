using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlockScript : MonoBehaviour
{
    [System.Serializable]
    public struct LevelColer //用來記錄每個數字級別對應到什麼顏色的結構
    {
        public int iLevel;
        public Color cBackgroundColor;
    }

    public Text sTextNum; //外頭顯示的字幕
    public LevelColer[] arrLevelColor; //每個數字級別分別該有顏色，存在該陣列裡
    private int iNum; //裡面儲存的數值

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetText(string str)
    {
        this.sTextNum.text = str;
    }
    public void SetText(int num)
    {
        this.iNum = num;
        this.ChangeColor();
        if(num != 0)
        {
            this.sTextNum.text = "" + num;
        }
        else
        {
            this.sTextNum.text = "";
        }
    }
    public void ChangeColor()
    {
        foreach(LevelColer levelColor in arrLevelColor)
        {
            if(this.iNum >= levelColor.iLevel)
            {
                this.GetComponent<Image>().color = levelColor.cBackgroundColor;
            }
        }
    }
}
