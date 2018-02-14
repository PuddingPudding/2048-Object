using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamingLogic2048
{
    public enum EPushingDirection
    {
        Left,
        Right,
        Up,
        Down,
        TopLeft,
        BottomLeft,
        TopRight,
        BottomRight
    }

    private List<BlockData> m_listBoard; //整個棋盤
    private int m_iScore; //分數
    private int m_iLengthX, m_iLengthY; //X軸邊長，和Y軸邊長
    private int m_iBoardSize; //整個棋盤的大小

    public GamingLogic2048(int _iLengthX, int _iLengthY, int _iScore) //最完善的建構子
    {
        this.m_iLengthX = _iLengthX;
        this.m_iLengthY = _iLengthY;
        this.m_iScore = _iScore;
        this.m_iBoardSize = m_iLengthX * m_iLengthY;
        this.m_listBoard = new List<BlockData>();

        for (int i = 0; i < m_iBoardSize; i++)
        {
            m_listBoard.Add(new BlockData());
        }
    }
    public GamingLogic2048() //最基本的遊戲開局
        : this(4, 4, 0)
    {
    }    

    public void SpawnBlock(int _iSpawnPos, BlockData _block) //生成格子
    {
        if (this.m_listBoard != null && _iSpawnPos < this.m_iBoardSize)//只在棋盤有被生出來，而且你選定的索引值在範圍內時才允許生成
        {
            this.m_listBoard[_iSpawnPos] = _block;
        }
    }

    public void SpawnRandomBlock(int _iSpawnNum)
    {
        List<BlockData> listEmptyBlock = new List<BlockData>(); //用來記憶哪些格子是空(0)的
        for (int i = 0; i < m_iBoardSize; i++)
        {
            if (m_listBoard[i].GetBlockType() == BlockData.EBlockType.Normal && m_listBoard[i].GetBlockValue() == 0) //只去找普通格子，判斷其是否為0
            {
                listEmptyBlock.Add(m_listBoard[i]);
            }
        }
        for (int i = 0; i < _iSpawnNum && listEmptyBlock.Count > 0; i++) //listEmptyBlock.Count > 0的用意是要檢查還有沒有空(為0)的格子
        {
            int iRandomValue = Random.Range(1, 3) * 2;
            int iBlockChose = Random.Range(0, listEmptyBlock.Count);
            listEmptyBlock[iBlockChose].SetBlockValue(iRandomValue);
            listEmptyBlock.RemoveAt(iBlockChose); //在設定完亂數後，將該選定之格子踢出列表，下一輪亂數就不會再出現了
        }
    }

    private List<List<int>> GetPushingList(EPushingDirection _ePushingDirection) //獲得正要進行 "壓縮" 的行線群，所以是list(行線)的list(群)之索引值
    {
        List<List<int>> listPushing = new List<List<int>>();
        switch (_ePushingDirection)
        {
            case EPushingDirection.Left:
                for (int i = 0; i < m_iLengthY; i++)
                {
                    List<int> listPushingLine = new List<int>();
                    for (int j = 0; j < m_iLengthX; j++)
                    {
                        int iChosen = (i * m_iLengthX) + j;
                        listPushingLine.Add(iChosen);
                    }
                    listPushing.Add(listPushingLine);
                }
                break;
            case EPushingDirection.Right:
                for (int i = 0; i < m_iLengthY; i++)
                {
                    List<int> listPushingLine = new List<int>();
                    for (int j = 0; j < m_iLengthX; j++)
                    {
                        int iChosen = ((i + 1) * m_iLengthX - 1) - j;
                        listPushingLine.Add(iChosen);
                    }
                    listPushing.Add(listPushingLine);
                }
                break;
            case EPushingDirection.Up:
                for (int i = 0; i < m_iLengthX; i++)
                {
                    List<int> listPushingLine = new List<int>();
                    for (int j = 0; j < m_iLengthY; j++)
                    {
                        int iChosen = i + (j * m_iLengthX);
                        listPushingLine.Add(iChosen);
                    }
                    listPushing.Add(listPushingLine);
                }
                break;
            case EPushingDirection.Down:
                for (int i = 0; i < m_iLengthX; i++)
                {
                    List<int> listPushingLine = new List<int>();
                    for (int j = 0; j < m_iLengthY; j++)
                    {
                        int iChosen = i + ((m_iLengthY - 1) - j) * m_iLengthX;
                        listPushingLine.Add(iChosen);
                    }
                    listPushing.Add(listPushingLine);
                }
                break;
            case EPushingDirection.TopLeft: //斜角方面，需要從起始點先往一邊(X軸方向)抓，再往下抓，所以需要兩個巢狀for
                for (int i = 0; i < m_iLengthX; i++) //先從0,0向右找
                {
                    List<int> listPushingLine = new List<int>();
                    for (int j = 0; j < (m_iLengthX - i); j++)
                    {
                        int iChosen = i + j + (j * m_iLengthX);
                        if (iChosen < m_iBoardSize && iChosen < (j + 1) * m_iLengthX)
                        {//前面的條件是不希望選中的格子超出整個棋盤範圍，後面的條件則是避免超出棋盤外圍的邊界
                            listPushingLine.Add(iChosen);
                        }
                    }
                    listPushing.Add(listPushingLine);
                }
                for (int i = 1; i < m_iLengthY; i++)//再從X,0(起點向下一格) 向下找
                {
                    List<int> listPushingLine = new List<int>();
                    for (int j = 0; j < (m_iLengthY - i); j++)
                    {
                        int iChosen = i * m_iLengthX + j + (j * m_iLengthX);
                        if (iChosen < m_iBoardSize && iChosen < (i + j + 1) * m_iLengthX)
                        {//前面的條件是不希望選中的格子超出整個棋盤範圍，後面的條件則是避免超出棋盤外圍的邊界
                            listPushingLine.Add(iChosen);
                        }
                    }
                    listPushing.Add(listPushingLine);
                }
                break;
            case EPushingDirection.BottomLeft: //往左下推，所以找格子時須往右上找
                for (int i = 0; i < m_iLengthX; i++) //先從0,0向右找
                {
                    List<int> listPushingLine = new List<int>();
                    for (int j = 0; j < (m_iLengthX - i); j++)
                    {
                        int iChosen = (m_iLengthY - 1) * m_iLengthX + i + j - (j * m_iLengthX);
                        if (iChosen < m_iBoardSize && iChosen < (m_iLengthY - j) * m_iLengthX)
                        {//前面的條件是不希望選中的格子超出整個棋盤範圍，後面的條件則是避免超出棋盤外圍的邊界
                            listPushingLine.Add(iChosen);
                        }
                    }
                    listPushing.Add(listPushingLine);
                }
                for (int i = 1; i < m_iLengthY; i++)
                {
                    List<int> listPushingLine = new List<int>();
                    for (int j = 0; j < (m_iLengthY - i); j++)
                    {
                        int iChosen = (m_iLengthY - 1 - i) * m_iLengthX + j - (j * m_iLengthX);
                        if (iChosen < m_iBoardSize && iChosen < (m_iLengthY - i - j) * m_iLengthX)
                        {//前面的條件是不希望選中的格子超出整個棋盤範圍，後面的條件則是避免超出棋盤外圍的邊界
                            listPushingLine.Add(iChosen);
                        }
                    }
                    listPushing.Add(listPushingLine);
                }
                break;
            case EPushingDirection.TopRight: //往右上推，所以找格子時須往左下找
                for (int i = 0; i < m_iLengthX; i++) //先從0,0向右找
                {
                    List<int> listPushingLine = new List<int>();
                    for (int j = 0; j <= i; j++)
                    {
                        int iChosen = i - j + (j * m_iLengthX);
                        if (iChosen < m_iBoardSize && iChosen >= j * m_iLengthX)
                        {
                            listPushingLine.Add(iChosen);
                        }
                    }
                    listPushing.Add(listPushingLine);
                }
                for (int i = 1; i < m_iLengthY; i++)
                {
                    List<int> listPushingLine = new List<int>();
                    for (int j = 0; j < (m_iLengthY - i); j++)
                    {
                        int iChosen = (i + 1) * m_iLengthX - 1 - j + (j * m_iLengthX);
                        if (iChosen < m_iBoardSize && iChosen >= (i + j) * m_iLengthX)
                        {
                            listPushingLine.Add(iChosen);
                        }
                    }
                    listPushing.Add(listPushingLine);
                }
                break;
            case EPushingDirection.BottomRight: //往右下推，所以找格子時須往左上找
                for (int i = 0; i < m_iLengthX; i++) //先從左下往右找
                {
                    List<int> listPushingLine = new List<int>();
                    for (int j = 0; j <= i; j++)
                    {
                        int iChosen = (m_iLengthY - 1) * m_iLengthX + i - j - (j * m_iLengthX);
                        if (iChosen < m_iBoardSize && iChosen >= (m_iLengthY - 1 - j) * m_iLengthX)
                        {
                            listPushingLine.Add(iChosen);
                        }
                    }
                    listPushing.Add(listPushingLine);
                }
                for (int i = 1; i < m_iLengthY; i++)
                {
                    List<int> listPushingLine = new List<int>();
                    for (int j = 0; j < (m_iLengthY - i); j++)
                    {
                        int iChosen = (m_iLengthY - i) * m_iLengthX - 1 - j - (j * m_iLengthX);
                        if (iChosen < m_iBoardSize && iChosen >= (m_iLengthY - i - 1 - j) * m_iLengthX)
                        {
                            listPushingLine.Add(iChosen);
                        }
                    }
                    listPushing.Add(listPushingLine);
                }
                break;
        }
        return listPushing;
    }

    public void Pushing(EPushingDirection _ePushingDirection) //壓縮動作，只需將 "方向" 帶入
    {
        List<List<int>> listPushing = this.GetPushingList(_ePushingDirection);
        foreach (List<int> listPushingLine in listPushing) //挑出行線群的其中一條
        {
            for (int i = 0; i < listPushingLine.Count; i++)//再針對該行線中的每個block做處理
            {
                BlockData blockComparing = m_listBoard[listPushingLine[i]]; //根據行線索引值找到該block物件
                switch (blockComparing.GetBlockType())
                {
                    case BlockData.EBlockType.Normal: //普通格子會朝行線的後面找到其他普通格子，並試著合併自己
                        bool bPolling = true; //表示正在輪尋
                        for (int j = i + 1; j < listPushingLine.Count && bPolling; j++)
                        {
                            BlockData blockPolling = m_listBoard[listPushingLine[j]];
                            switch (blockPolling.GetBlockType())
                            {
                                case BlockData.EBlockType.Normal://當遇到普通格子時，檢察其值是否不為0
                                    if (blockPolling.GetBlockValue() != 0)
                                    {
                                        if (blockComparing.GetBlockValue() == 0)
                                        {
                                            blockComparing.PlusOn(blockPolling);
                                        }
                                        else if (blockComparing.GetBlockValue() == blockPolling.GetBlockValue())
                                        {
                                            blockComparing.PlusOn(blockPolling);
                                            this.m_iScore += blockComparing.GetBlockValue();
                                            bPolling = false;
                                        }
                                        else if (blockComparing.GetBlockValue() != blockPolling.GetBlockValue())
                                        {
                                            bPolling = false;
                                        }
                                    }
                                    break;
                                case BlockData.EBlockType.Obstruct://當普通格子遇到障礙時，停止輪尋
                                    bPolling = false;
                                    break;
                                case BlockData.EBlockType.None: //當普通格子遇到空洞，直接跳過，但不會停止輪尋
                                    break;
                            }
                        }
                        break;
                    case BlockData.EBlockType.Obstruct: //障礙格子什麼都不會做，直接跳過
                        break;
                    case BlockData.EBlockType.None: //空洞格子什麼也不會做，直接跳過
                        break;
                }
            }
        }
    }

    public bool GetPushAble(EPushingDirection _ePushingDirection)
    {
        List<List<int>> listPushing = this.GetPushingList(_ePushingDirection);
        bool bPushAble = false;//是否可進行壓縮
        //Debug.Log("行線群的數目 " + listPushing.Count);
        for (int i = 0; i < listPushing.Count && !bPushAble; i++) //挑出行線群的每一條行線進行檢查，若中途發現可以進行壓縮的話就跳出迴圈，因為沒有繼續檢查下去的必要
        {
            for (int j = 0; j < listPushing[i].Count - 1 && !bPushAble; j++)
            {
                BlockData blockComparing = m_listBoard[listPushing[i][j]]; //根據行線索引值找到該block物件
                switch (blockComparing.GetBlockType())
                {
                    case BlockData.EBlockType.Normal: //普通格子就會去檢查自己的下一格是否也是普通格子，且為0或跟自己相等
                        int iNext = j + 1; //用來找出下一格的索引值
                        BlockData blockPolling = m_listBoard[listPushing[i][iNext]];
                        while (blockPolling.GetBlockType() == BlockData.EBlockType.None && iNext < listPushing[i].Count-1) //空洞格子雖然沒有效果，但是普通格子可以越過，所以判斷能不能推動時，必須跳過
                        {
                            iNext++;
                            blockPolling = m_listBoard[listPushing[i][iNext]];
                        } //一路一直找，直到找到非空洞(None的格子)，或著找到底了為止
                        if (blockPolling.GetBlockType() == BlockData.EBlockType.Normal)
                        {
                            //Debug.Log("Comparing(" + listPushing[i][j] + "):" + blockComparing.GetBlockValue() + " Polling(" + listPushing[i][iNext] + "):" + blockPolling.GetBlockValue());
                            if (blockComparing.GetBlockValue() == 0 && blockPolling.GetBlockValue() != 0)
                            {
                                bPushAble = true;
                            }
                            else if (blockComparing.GetBlockValue() != 0 && blockComparing.GetBlockValue() == blockPolling.GetBlockValue())
                            {
                                bPushAble = true;
                            }
                        }
                        break;
                    case BlockData.EBlockType.Obstruct: //障礙格子什麼都不會做，直接跳過
                        break;
                    case BlockData.EBlockType.None: //空洞格子什麼也不會做，直接跳過
                        break;
                }
            }
        }
        return bPushAble;
    }

    public int GetBoardSize()
    {
        return this.m_iBoardSize;
    }
    public List<BlockData> GetBoardData()
    {
        return this.m_listBoard.GetRange(0, m_listBoard.Count); //回傳list本身的複製品，因為要避免別人透過Get來直接更動list中的值
    }
    public int GetScore()
    {
        return this.m_iScore;
    }
}
