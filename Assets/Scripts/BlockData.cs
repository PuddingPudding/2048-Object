using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockData
{
    public enum EBlockType //格子種類
    {
        Normal, //普通
        Obstruct, //障礙物(不可越過)
        None //沒東西(類似障礙物，但可以越過)
    }

    private EBlockType m_eBlockType; //自己的格子種類
    private int m_iBlockValue; //自己所帶的數值
    private BlockData[] m_arrNeighbor; //自己周圍的其他格子

    public bool Conbinable //得知自己是否是可以進行結合動作的方塊類型，未來若出現新的可結合之格子種類，只要在下面補上就好，唯讀
    {
        get { return (this.m_eBlockType == EBlockType.Normal); }
    }

    public BlockData(EBlockType _eBlockType, int _iBlockValue)
    {
        this.m_eBlockType = _eBlockType;
        this.m_iBlockValue = _iBlockValue;
        int iNumber = System.Enum.GetNames(typeof(GamingLogic2048.EPushingDirection)).Length;//取得邏輯層所擁有的 "推動方向" 數量
        m_arrNeighbor = new BlockData[iNumber];
    }
    public BlockData(EBlockType _eBlockType)
        : this(_eBlockType, 0) //假如只給格子種類的話就預設數值為0
    {
    }
    public BlockData()
        : this(EBlockType.Normal, 0) //假如什麼都不設定就假設自己是普通格子，且數值為0
    {
    }

    public void PlusOn(BlockData _anotherBlock)
    {
        this.m_iBlockValue += _anotherBlock.m_iBlockValue;
        _anotherBlock.m_iBlockValue = 0;
    }

    public bool TryToCombine(BlockData _anotherBlock)
    {
        bool bCanCombine = false;
        if (_anotherBlock != null && this.m_eBlockType == EBlockType.Normal) //只有普通格子能夠跟其他格子檢查是否可結合
        {

        }
        return bCanCombine;
    }

    public bool TryToCombine(GamingLogic2048.EPushingDirection _direction)
    {
        bool bCanCombine = false;
        BlockData anotherBlock = this.GetNeighbor(_direction);
        if (anotherBlock != null)
        {
            switch (anotherBlock.m_eBlockType)
            {
                case EBlockType.Normal:
                    bCanCombine = true;
                    break;
                case EBlockType.Obstruct:
                    bCanCombine = false;
                    break;
                case EBlockType.None:
                    bCanCombine = anotherBlock.TryToCombine(_direction);
                    break;
            }
        }
        return bCanCombine;
    }

    public BlockData GetNextCombinableBlock(GamingLogic2048.EPushingDirection _direction) //獲得某個方位上的下一個可結合之格子，沒辦法的話就回傳null
    {
        BlockData nextCombinableBlock = null;
        BlockData nextBlock = this.GetNeighbor(_direction);
        if (nextBlock != null)
        {
            if(nextBlock.Conbinable)
            {
                nextCombinableBlock = nextBlock;
            }
            else
            {
                switch (nextBlock.m_eBlockType)
                {
                    case EBlockType.Obstruct: //障礙格子什麼也不做，自然的讓函式回傳null
                        nextCombinableBlock = null;
                        break;
                    case EBlockType.None: //空洞格子會一路繼續找下去，直到找到底或著下個可判斷的格子為止
                        nextCombinableBlock = nextBlock.GetNextCombinableBlock(_direction);
                        break;
                }
            }
        }
        return nextCombinableBlock;
    }

    public void SetBlockType(EBlockType _newType)
    {
        this.m_eBlockType = _newType;
    }
    public EBlockType GetBlockType()
    {
        return this.m_eBlockType;
    }
    //我感覺如果只是這樣寫的話，那似乎也不用get和set，所以有些猶豫
    public void SetBlockValue(int _iNewValue)
    {
        this.m_iBlockValue = _iNewValue;
    }
    public int GetBlockValue()
    {
        return this.m_iBlockValue;
    }
    public void SetNeighbor(BlockData _anotherBlock, GamingLogic2048.EPushingDirection _direction)
    {
        int iThisToNeighbor = (int)_direction; //自己看鄰居的方向
        int iNeighborToThis = 0; //鄰居看自己的方向
        switch (_direction)
        {
            case GamingLogic2048.EPushingDirection.Left:
                iNeighborToThis = (int)GamingLogic2048.EPushingDirection.Right;
                break;
            case GamingLogic2048.EPushingDirection.Right:
                iNeighborToThis = (int)GamingLogic2048.EPushingDirection.Left;
                break;
            case GamingLogic2048.EPushingDirection.Up:
                iNeighborToThis = (int)GamingLogic2048.EPushingDirection.Down;
                break;
            case GamingLogic2048.EPushingDirection.Down:
                iNeighborToThis = (int)GamingLogic2048.EPushingDirection.Up;
                break;
            case GamingLogic2048.EPushingDirection.TopLeft:
                iNeighborToThis = (int)GamingLogic2048.EPushingDirection.BottomRight;
                break;
            case GamingLogic2048.EPushingDirection.BottomLeft:
                iNeighborToThis = (int)GamingLogic2048.EPushingDirection.TopRight;
                break;
            case GamingLogic2048.EPushingDirection.TopRight:
                iNeighborToThis = (int)GamingLogic2048.EPushingDirection.BottomLeft;
                break;
            case GamingLogic2048.EPushingDirection.BottomRight:
                iNeighborToThis = (int)GamingLogic2048.EPushingDirection.TopLeft;
                break;
        }
        this.m_arrNeighbor[iThisToNeighbor] = _anotherBlock;
        _anotherBlock.m_arrNeighbor[iNeighborToThis] = this;
    }
    public BlockData GetNeighbor(GamingLogic2048.EPushingDirection _direction)
    {
        int iThisToNeighbor = (int)_direction; //自己看鄰居的方向
        return this.m_arrNeighbor[iThisToNeighbor];
    }
    public BlockData GetNeighbor(int _iDirection)
    {
        return this.m_arrNeighbor[_iDirection];
    }
}