using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewControll : MonoBehaviour
{
    public enum EGamingMode
    {
        Customize, //自訂
        Standard, //標準模式，4*4
        Small, //小格局，3*3
        Big, //大格局，5*5
        BigAndObstruct, //大而阻塞，5*5，並且放入4個障礙物，分別在6 8 16 18 的位子
        BigAndNone, //大而空洞，將上面那個模式的障礙物改為可越過的空洞
        Insane //瘋狂模式，6*6並且會隨機生成3個障礙物跟3個空洞
    }

    [SerializeField] private Transform m_transformInitPosition; //第一個生成格子物件的位子
    [SerializeField] private GameObject m_gameObjBlockPrefab; //每個格子的遊戲物件原型
    [SerializeField] private Vector2 m_vec2BlockInterval = new Vector2(60, 60); //每個格子物件間的間隔
    [SerializeField] private Text m_textScore;//計分表
    [SerializeField] private int m_iLengthX = 4, m_iLengthY = 4; //棋盤邊長
    [SerializeField] private EGamingMode m_eGameMode = EGamingMode.Customize;
    [SerializeField] private InputField m_inputLengthX; //輸入欄位，讓玩家在遊戲結束時可以自己手動輸入，決定邊長X
    [SerializeField] private InputField m_inputLengthY; //輸入欄位，讓玩家在遊戲結束時可以自己手動輸入，決定邊長Y
    [SerializeField] private Dropdown m_dropdownGameMode; //下拉式選單，讓玩家知道自己選擇要玩什麼模式
    [SerializeField] private GameObject m_gameObjGameOverView; //下拉式選單，讓玩家知道自己選擇要玩什麼模式
    private List<BlockView> m_listBlockView; //用來抓到每個棋盤內格子物件的程式碼
    private GamingLogic2048 m_game; //遊戲物件，負責真正的管理遊戲邏輯與資料
    private int[] m_iArrSpecialBlockPos = new int[] { 6, 8, 16, 18 };
    private bool m_bIsGaming = false;

    // Use this for initialization
    void Start()
    {
        m_listBlockView = new List<BlockView>();
        InitGame(m_eGameMode);
        UpdateGame();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.Keypad4))
        {
            if (m_game.GetPushAble(GamingLogic2048.EPushingDirection.Left))
            {
                m_game.Pushing(GamingLogic2048.EPushingDirection.Left);
                m_game.SpawnRandomBlock(1);
                UpdateGame();
            }
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.Keypad6))
        {
            if (m_game.GetPushAble(GamingLogic2048.EPushingDirection.Right))
            {
                m_game.Pushing(GamingLogic2048.EPushingDirection.Right);
                m_game.SpawnRandomBlock(1);
                UpdateGame();
            }
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.Keypad8))
        {
            if (m_game.GetPushAble(GamingLogic2048.EPushingDirection.Up))
            {
                m_game.Pushing(GamingLogic2048.EPushingDirection.Up);
                m_game.SpawnRandomBlock(1);
                UpdateGame();
            }
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.Keypad2))
        {
            if (m_game.GetPushAble(GamingLogic2048.EPushingDirection.Down))
            {
                m_game.Pushing(GamingLogic2048.EPushingDirection.Down);
                m_game.SpawnRandomBlock(1);
                UpdateGame();
            }
        }
        else if (Input.GetKeyDown(KeyCode.Keypad7))
        {
            if (m_game.GetPushAble(GamingLogic2048.EPushingDirection.TopLeft))
            {
                m_game.Pushing(GamingLogic2048.EPushingDirection.TopLeft);
                m_game.SpawnRandomBlock(1);
                UpdateGame();
            }
        }
        else if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            if (m_game.GetPushAble(GamingLogic2048.EPushingDirection.BottomLeft))
            {
                m_game.Pushing(GamingLogic2048.EPushingDirection.BottomLeft);
                m_game.SpawnRandomBlock(1);
                UpdateGame();
            }
        }
        else if (Input.GetKeyDown(KeyCode.Keypad9))
        {
            if (m_game.GetPushAble(GamingLogic2048.EPushingDirection.TopRight))
            {
                m_game.Pushing(GamingLogic2048.EPushingDirection.TopRight);
                m_game.SpawnRandomBlock(1);
                UpdateGame();
            }
        }
        else if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            if (m_game.GetPushAble(GamingLogic2048.EPushingDirection.BottomRight))
            {
                m_game.Pushing(GamingLogic2048.EPushingDirection.BottomRight);
                m_game.SpawnRandomBlock(1);
                UpdateGame();
            }
        }

        m_bIsGaming = false;
        foreach (GamingLogic2048.EPushingDirection eDirection in System.Enum.GetValues(typeof(GamingLogic2048.EPushingDirection)) )
        {
            if(m_game.GetPushAble(eDirection) ) //只要其中一個方向可以過的話，就依舊先判定還可遊玩
            {
                m_bIsGaming = true;
            }
        }
        if(!m_bIsGaming)
        {
            this.m_gameObjGameOverView.SetActive(true);
        }
    }

    private void BuildBoard(int _iLengthX , int _iLengthY) //單純的把棋盤建出來，裡頭都先不要設定數字或障礙
    {
        m_game = new GamingLogic2048(_iLengthX, _iLengthY, 0);
        int iBoardSize = m_game.GetBoardSize();
        for (int i = 0; i < iBoardSize || i < m_listBlockView.Count ; i++) //為實現資源管理，迴圈需要兩個條件，一個是生成格子時是否生足夠了，另一個則是去找原本的格子是否太多了，需要關閉
        {
            if(i >= m_listBlockView.Count) //超出了現有生成的格子範圍，表示需要額外生成
            {
                Vector3 initPlace = Vector3.zero;
                initPlace.x += m_vec2BlockInterval.x * (i % _iLengthX);
                initPlace.y -= m_vec2BlockInterval.y * (i / _iLengthX);
                GameObject blockTemp = Instantiate(m_gameObjBlockPrefab, initPlace, m_gameObjBlockPrefab.transform.rotation);
                blockTemp.transform.SetParent(m_transformInitPosition.transform);
                blockTemp.transform.localPosition = initPlace;
                BlockView BVTemp = blockTemp.GetComponent<BlockView>();
                m_listBlockView.Add(BVTemp);
            }
            else if(i < iBoardSize && i < m_listBlockView.Count) //還需要格子，但原本所生成的格子還夠用時，將舊有的格子移動到我們需要的地方
            {
                Vector3 initPlace = Vector3.zero;
                initPlace.x += m_vec2BlockInterval.x * (i % _iLengthX);
                initPlace.y -= m_vec2BlockInterval.y * (i / _iLengthX);
                m_listBlockView[i].transform.localPosition = initPlace;
                m_listBlockView[i].transform.gameObject.SetActive(true);
            }
            else if(i >= iBoardSize && i < m_listBlockView.Count)
            {
                m_listBlockView[i].transform.gameObject.SetActive(false);
            }
        }

        this.m_gameObjGameOverView.SetActive(false);
    }
    private void InitGame(EGamingMode _eGameMode)
    {
        switch(_eGameMode)
        {
            case EGamingMode.Customize:
                BuildBoard(m_iLengthX, m_iLengthY);
                break;
            case EGamingMode.Standard:
                BuildBoard(4, 4);
                break;
            case EGamingMode.Small:
                BuildBoard(3, 3);
                break;
            case EGamingMode.Big:
                BuildBoard(5 , 5);
                break;
            case EGamingMode.BigAndObstruct:
                BuildBoard(5, 5);
                for(int i = 0; i< m_iArrSpecialBlockPos.Length; i++)
                {
                    m_game.SpawnBlock(m_iArrSpecialBlockPos[i], new BlockData(BlockData.EBlockType.Obstruct));
                }
                break;
            case EGamingMode.BigAndNone:
                BuildBoard(5, 5);
                for (int i = 0; i < m_iArrSpecialBlockPos.Length; i++)
                {
                    m_game.SpawnBlock(m_iArrSpecialBlockPos[i], new BlockData(BlockData.EBlockType.None));
                }
                break;
            case EGamingMode.Insane:
                BuildBoard(6 , 6);
                for (int i = 0; i < 3; i++)
                {
                    m_game.SpawnBlock(Random.Range(0, m_game.GetBoardSize()), new BlockData(BlockData.EBlockType.Obstruct));
                }
                for (int i = 0; i < 3; i++)
                {
                    m_game.SpawnBlock(Random.Range(0,m_game.GetBoardSize() ), new BlockData(BlockData.EBlockType.None));
                }
                break;
        }
        m_game.SpawnRandomBlock(2);
        m_bIsGaming = true;
    }
    public void UpdateGame()
    {
        int iBoardSize = m_game.GetBoardSize();
        List<BlockData> listBoardData = m_game.GetBoardData();
        for (int i = 0; i < iBoardSize; i++)
        {
            m_listBlockView[i].SetBlock(listBoardData[i]);
        }
        m_textScore.text = "score: " + m_game.GetScore();
    }
    public void InitGameOnClick()
    {
        switch(m_dropdownGameMode.value)
        {
            case 0:
                m_eGameMode = EGamingMode.Customize;
                int iNewLengthX = System.Convert.ToInt32(m_inputLengthX.text); //原本使用的是int.parse()的方法，然而int.parse似乎無法處理null字串
                int iNewLengthY = System.Convert.ToInt32(m_inputLengthY.text); //System.Conver.ToInt32會將null字串轉為0來做處理
                if (iNewLengthX >= 2 && iNewLengthX <= 5) //若新的邊長在合理範圍，則直接設定
                {
                    m_iLengthX = iNewLengthX;
                }//而其餘的情況則乾脆不變
                if (iNewLengthY >= 2 && iNewLengthY <= 5) //若新的邊長在合理範圍，則直接設定
                {
                    m_iLengthY = iNewLengthY;
                }//而其餘的情況則乾脆不變
                break;
            case 1:
                m_eGameMode = EGamingMode.Standard;
                break;
            case 2:
                m_eGameMode = EGamingMode.Small;
                break;
            case 3:
                m_eGameMode = EGamingMode.Big;
                break;
            case 4:
                m_eGameMode = EGamingMode.BigAndObstruct;
                break;
            case 5:
                m_eGameMode = EGamingMode.BigAndNone;
                break;
            case 6:
                m_eGameMode = EGamingMode.Insane;
                break;
        }
        this.InitGame(m_eGameMode);
        this.UpdateGame();
    }

}
