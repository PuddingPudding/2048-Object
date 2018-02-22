using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamingLogic2048
{
    public enum EPushingDirection //推動的方向，特別注意，方向必須以互為相反的排在一起，兩兩成對，因為這樣才方便讓函式取得相反方向
    {
        Left,
        Right,
        Up,
        Down,
        TopLeft,
        BottomRight,
        TopRight,
        BottomLeft
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
        for (int i = 0; i < m_iLengthY; i++)
        {
            for (int j = 0; j < m_iLengthX; j++)
            {
                int iSetting = i * m_iLengthX + j; //依照順序找出各別需要設定的格子，接著再去依照八個方位設定關係
                int iNeighbor; //用來記錄周遭八方位之座標

                iNeighbor = iSetting - 1;
                if (iNeighbor >= i * m_iLengthX)
                {
                    m_listBoard[iSetting].SetNeighbor(m_listBoard[iNeighbor], EPushingDirection.Left);
                }
                iNeighbor = iSetting + 1;
                if (iNeighbor < (i + 1) * m_iLengthX)
                {
                    m_listBoard[iSetting].SetNeighbor(m_listBoard[iNeighbor], EPushingDirection.Right);
                }
                iNeighbor = iSetting - m_iLengthX;
                if (iNeighbor >= 0)
                {
                    m_listBoard[iSetting].SetNeighbor(m_listBoard[iNeighbor], EPushingDirection.Up);
                }
                iNeighbor = iSetting + m_iLengthX;
                if (iNeighbor < m_iBoardSize)
                {
                    m_listBoard[iSetting].SetNeighbor(m_listBoard[iNeighbor], EPushingDirection.Down);
                }
                iNeighbor = iSetting - m_iLengthX - 1;
                if (iNeighbor >= (i - 1) * m_iLengthX && iNeighbor >= 0)
                {
                    m_listBoard[iSetting].SetNeighbor(m_listBoard[iNeighbor], EPushingDirection.TopLeft);
                }
                iNeighbor = iSetting + m_iLengthX + 1;
                if (iNeighbor < (i + 1) * m_iLengthX && iNeighbor < m_iBoardSize)
                {
                    m_listBoard[iSetting].SetNeighbor(m_listBoard[iNeighbor], EPushingDirection.BottomRight);
                }
                iNeighbor = iSetting - m_iLengthX + 1;
                if (iNeighbor < i * m_iLengthX && iNeighbor >= 0)
                {
                    m_listBoard[iSetting].SetNeighbor(m_listBoard[iNeighbor], EPushingDirection.TopRight);
                }
                iNeighbor = iSetting + m_iLengthX - 1;
                if (iNeighbor >= (i + 1) * m_iLengthX && iNeighbor < m_iBoardSize)
                {
                    m_listBoard[iSetting].SetNeighbor(m_listBoard[iNeighbor], EPushingDirection.BottomLeft);
                }
            }
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
            this.m_listBoard[_iSpawnPos].SetBlockValue(_block.GetBlockValue());
            this.m_listBoard[_iSpawnPos].SetBlockType(_block.GetBlockType());
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

    private List<List<BlockData>> GetPushingList(EPushingDirection _ePushingDirection) //獲得正要進行 "壓縮" 的行線群，所以是list(行線)的list(群)
    {
        List<List<BlockData>> listPushing = new List<List<BlockData>>();
        List<int> listPushingPoint = this.GetPushingPoint(_ePushingDirection);
        EPushingDirection eCombineDir = GetOppositeDirection(_ePushingDirection);
        foreach (int iStartPoint in listPushingPoint)
        {
            BlockData blockPick = m_listBoard[iStartPoint];
            List<BlockData> pushingLine = new List<BlockData>
            {
                blockPick
            };
            while (blockPick.GetNextCombinableBlock(eCombineDir) != null)
            {
                blockPick = blockPick.GetNextCombinableBlock(eCombineDir);
                pushingLine.Add(blockPick);
            }
            listPushing.Add(pushingLine);
        }
        return listPushing;
    }

    private List<int> GetPushingPoint(EPushingDirection _eDirection) //獲得所有該壓縮方向之起點
    {
        List<int> listPushingPoint = new List<int>();
        for (int i = 0; i < m_iBoardSize; i++)
        {
            if (m_listBoard[i].Conbinable && m_listBoard[i].GetNextCombinableBlock(_eDirection) == null)
            {//往某方向壓縮的起點之定義，必須要是可結合的格子(普通)，同時，他也是該方向之底，所以往該方向搜索時他找不到可結合的東西
                listPushingPoint.Add(i);
            }
        }
        return listPushingPoint;
    }

    public void Pushing(EPushingDirection _ePushingDirection) //壓縮動作，只需將 "方向" 帶入
    {
        List<List<BlockData>> listPushing = this.GetPushingList(_ePushingDirection);
        foreach (List<BlockData> listPushingLine in listPushing) //挑出行線群的其中一條
        {
            for (int i = 0; i < listPushingLine.Count; i++)
            {
                BlockData blockComparing = listPushingLine[i];
                bool bPolling = true; //表示正在輪尋
                for (int j = i + 1; j < listPushingLine.Count && bPolling; j++)
                {
                    BlockData blockPolling = listPushingLine[j];
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
                }
            }
        }
    }

    public bool GetPushAble(EPushingDirection _ePushingDirection)
    {
        List<List<BlockData>> listPushing = this.GetPushingList(_ePushingDirection);
        bool bPushAble = false;//是否可進行壓縮
        for (int i = 0; i < listPushing.Count && !bPushAble; i++) //挑出行線群的每一條行線進行檢查，若中途發現可以進行壓縮的話就跳出迴圈，因為沒有繼續檢查下去的必要
        {
            for (int j = 0; j < listPushing[i].Count - 1 && !bPushAble; j++)
            {
                BlockData blockComparing = listPushing[i][j]; //根據行線索引值找到該block物件
                BlockData blockPolling = listPushing[i][j + 1];
                if (blockComparing.GetBlockValue() == 0 && blockPolling.GetBlockValue() != 0)
                {
                    bPushAble = true;
                }
                else if (blockComparing.GetBlockValue() != 0 && blockComparing.GetBlockValue() == blockPolling.GetBlockValue())
                {
                    bPushAble = true;
                }
            }
        }
        return bPushAble;
    }

    public static EPushingDirection GetOppositeDirection(EPushingDirection _eDirection)
    {
        EPushingDirection oppositeDirection = 0;
        if ((int)_eDirection % 2 == 0)
        {
            oppositeDirection = _eDirection + 1;
        }
        else
        {
            oppositeDirection = _eDirection - 1;
        }
        return oppositeDirection;
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