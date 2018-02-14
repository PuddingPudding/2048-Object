using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlockView : MonoBehaviour
{
    [System.Serializable]
    public struct LevelColor //用來記錄每個數字級別對應到什麼顏色的結構 (只在Normal類的格子中使用)
    {
        public int iLevel;
        public Color colorBackgroundColor;
    }

    [SerializeField] private Sprite m_sprNormalFace;
    [SerializeField] private Sprite m_sprObstructFace;
    [SerializeField] private Sprite m_sprNoneFace;
    [SerializeField] private LevelColor[] m_arrLevelColor;
    [SerializeField] private Text m_textNum; //外頭顯示的字幕
    [SerializeField]private Image m_imgBlockFace; //格子的外觀

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetBlock(BlockData _data)
    {
        switch(_data.GetBlockType() )
        {
            case BlockData.EBlockType.Normal:
                this.m_imgBlockFace.sprite = m_sprNormalFace;
                this.SetText(_data.GetBlockValue());
                break;
            case BlockData.EBlockType.Obstruct:
                this.m_imgBlockFace.sprite = m_sprObstructFace;
                this.SetText(0);
                break;
            case BlockData.EBlockType.None:
                this.m_imgBlockFace.sprite = m_sprNoneFace;
                this.SetText(0);
                break;
        }
    }

    public void SetText(string _sText)
    {
        this.m_textNum.text = _sText;
    }
    public void SetText(int _iNum)
    {
        this.ChangeColor(_iNum);
        if (_iNum != 0)
        {
            this.m_textNum.text = "" + _iNum;
        }
        else
        {
            this.m_textNum.text = "";
        }
    }
    public void SetText(BlockData _data)
    {
        if(_data.GetBlockType() == BlockData.EBlockType.Normal) //只有當我發現你是普通格子時，我才會去更動我的數字
        {
            this.SetText(_data.GetBlockValue());
        }
    }
    public void ChangeColor(int _iNum)
    {
        foreach (LevelColor levelColor in m_arrLevelColor)
        {
            if (_iNum >= levelColor.iLevel)
            {
                this.m_imgBlockFace.color = levelColor.colorBackgroundColor;
            }
        }
    }
}
