
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public int iLength = 4; //棋盤邊長
    public GameObject blockPrefab; //每個格子的遊戲物件原型
    public Vector2 v2BlockInterval = new Vector2(60, 60); //每個格子物件間的間隔
    public Transform tInitPosition; //第一個生成格子物件的位子
    public Text textScore;
    public GameObject gameOverView;//失敗時會看到的東西
    public InputField textLengthInput; //輸入欄位，讓玩家在遊戲結束時可以自己手動輸入，決定邊長

    private List<int> iListCheckBoard; //用來紀錄整個2048棋盤中每一格數字的串列
    private List<BlockScript> listBlockScript;//用來存取每一個格子的程式碼，格子身上的程式碼可以讓你設定顯示的數字
    private int iScore = 0;
    // Use this for initialization
    void Start()
    {
        textLengthInput.text = "" + iLength;
        InitGame();
        UpdateGame();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (this.PushAbleLeft())
            {
                this.PushLeft();
                this.SpawnBlock(1);
            }
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (this.PushAbleRight())
            {
                this.PushRight();
                this.SpawnBlock(1);
            }
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (this.PushAbleUp())
            {
                this.PushUp();
                this.SpawnBlock(1);
            }
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (this.PushAbleDown())
            {
                this.PushDown();
                this.SpawnBlock(1);
            }
        }

        UpdateGame();

        if (!PushAbleDown() && !PushAbleLeft() && !PushAbleRight() && !PushAbleUp())
        {
            this.gameOverView.SetActive(true);
        }
    }

    public void InitGame()
    {
        int newLength = System.Convert.ToInt32(textLengthInput.text); //原本使用的是int.parse()的方法，然而int.parse似乎無法處理null字串
        if(newLength >= 2 && newLength <= 5) //若新的邊長在合理範圍，則直接設定
        {
            iLength = newLength;
        }//而其餘的情況則乾脆不變

        if (iListCheckBoard == null && listBlockScript == null)//假設兩者(數字陣列與格子程式的陣列)都尚未生成，則幫他們新分配
        {
            iListCheckBoard = new List<int>();
            listBlockScript = new List<BlockScript>();
            for (int i = 0; i < (iLength * iLength); i++)
            {
                iListCheckBoard.Add(0);
                Vector3 initPlace = Vector3.zero;
                initPlace.x += v2BlockInterval.x * (i % iLength);
                initPlace.y -= v2BlockInterval.y * (i / iLength);
                GameObject blockTemp = Instantiate(blockPrefab, initPlace, blockPrefab.transform.rotation);
                blockTemp.transform.SetParent(tInitPosition.transform);
                blockTemp.transform.localPosition = initPlace;
                BlockScript BSTemp = blockTemp.GetComponent<BlockScript>();
                listBlockScript.Add(BSTemp);
            }
        }
        else //其餘狀況，也就是已經有先生成過了，只是要重玩
        {
            for (int i = 0; i < (iLength * iLength) || i < listBlockScript.Count; i++)
            {//這裡的迴圈判斷條件有兩個，因為如果新的棋盤較大的話，那原本的list就需要擴增，但如果新棋盤較小的話，那原本list多出來的部分就需要先關閉(但不要刪掉)

                if (i < (iLength * iLength) && i < listBlockScript.Count) //假如目前新格子的編號還在原本的list範圍內的話
                {
                    iListCheckBoard[i] = 0;
                    Vector3 initPlace = Vector3.zero;
                    initPlace.x += v2BlockInterval.x * (i % iLength);
                    initPlace.y -= v2BlockInterval.y * (i / iLength);
                    listBlockScript[i].gameObject.SetActive(true);
                    listBlockScript[i].transform.SetParent(tInitPosition.transform);
                    listBlockScript[i].transform.localPosition = initPlace;
                }
                else if (i < (iLength * iLength) && i >= listBlockScript.Count) //假如目前新格子的編號已經超出，則需要額外再生成格子物件給新棋盤
                {
                    iListCheckBoard.Add(0);
                    Vector3 initPlace = Vector3.zero;
                    initPlace.x += v2BlockInterval.x * (i % iLength);
                    initPlace.y -= v2BlockInterval.y * (i / iLength);
                    GameObject blockTemp = Instantiate(blockPrefab, initPlace, blockPrefab.transform.rotation);
                    blockTemp.transform.SetParent(tInitPosition.transform);
                    blockTemp.transform.localPosition = initPlace;
                    BlockScript BSTemp = blockTemp.GetComponent<BlockScript>();
                    listBlockScript.Add(BSTemp);
                }
                else if (i >= (iLength * iLength) && i < listBlockScript.Count) //假如目前的編號已經超出了新格子的需求，那就將原本list多出來的物件先關閉
                {
                    listBlockScript[i].gameObject.SetActive(false);
                }
            }
        }
        iScore = 0;
        gameOverView.SetActive(false);
        SpawnBlock(2);
    }

    public void SpawnBlock(int iSpawnNum)
    {
        for (int i = 0; i < iSpawnNum; i++)
        {
            int iBlockNum = Random.Range(1, 3) * 2;
            int iBlockChose = Random.Range(0, (iLength * iLength));
            bool canSpawn = false;
            for (int j = 0; j < (iLength * iLength) && !canSpawn; j++)
            {
                if (iListCheckBoard[j] == 0)
                {
                    canSpawn = true; //如果陣列裡還有空值(0)的話，就設定為可以生成
                }
            }
            while (iListCheckBoard[iBlockChose] != 0 && canSpawn) //一直跑回圈檢察，直到隨機挑出一個還未有數字(0)的空格為止
            {
                iBlockChose = Random.Range(0, iListCheckBoard.Count);
            }
            if (canSpawn)
            {
                iListCheckBoard[iBlockChose] = iBlockNum;
            }
        }
    }

    public void UpdateGame()
    {
        for (int i = 0; i < (iLength * iLength); i++) //改寫為棋盤邊長的平方，因為未來如果玩家先生成了一個4*4的棋盤，而後又改玩2*2的話，那裝數字的列表就會變得過長
        {
            listBlockScript[i].SetText(iListCheckBoard[i]);
        }
        textScore.text = "score: " + this.iScore;
    }

    public void PushLeft()
    {
        int iComparing; //正在受比對的格子索引值
        int iPolling; //輪尋格子的索引值
        bool pollFinish = false; //這一波輪尋是否已結束
        for (int i = 0; i < iLength; i++)
        {
            for (int j = 0; j < iLength; j++)
            {
                pollFinish = false;
                iComparing = (i * iLength) + j;
                iPolling = iComparing + 1;
                while (!pollFinish)
                {
                    if (iPolling >= (i + 1) * iLength)//若輪尋格子超出了目前的這條行線
                    {
                        pollFinish = true;
                    }
                    else if (iListCheckBoard[iPolling] != 0)//若輪尋格子有東西(不為0)
                    {
                        if (iListCheckBoard[iComparing] == 0)//但受比對的格子是空的(0)的話，那就直接把數字搬過來
                        {
                            iListCheckBoard[iComparing] = iListCheckBoard[iPolling];
                            iListCheckBoard[iPolling] = 0;
                        }
                        else //其餘狀況，也就是當受比對的格子有東西(不為0)
                        {
                            if (iListCheckBoard[iComparing] == iListCheckBoard[iPolling])//若比對者與輪尋者皆同，兩者合併
                            {
                                iListCheckBoard[iComparing] += iListCheckBoard[iPolling];
                                this.iScore += iListCheckBoard[iComparing];
                                iListCheckBoard[iPolling] = 0;
                            }
                            else if (iListCheckBoard[iComparing + 1] == 0)//若比對者的隔壁沒東西(0)，那就把輪尋到的那個數字直接搬過來
                            {
                                iListCheckBoard[iComparing + 1] += iListCheckBoard[iPolling];
                                iListCheckBoard[iPolling] = 0;
                            }
                            pollFinish = true;
                        }
                    }
                    else//其餘狀況，也就是當輪尋格子沒東西(0)，且輪尋值也還未到底時
                    {
                        iPolling++;
                    }
                }
            }
        }
    }

    public bool PushAbleLeft()
    {
        bool pushAble = false; //是否能推
        int iChecking;//檢查用的格子，從最開始(第0個)檢查到最後
        for (int i = 0; i < iLength && !pushAble; i++)
        {
            for (int j = 0; j < iLength - 1 && !pushAble; j++)
            {
                iChecking = (i * iLength) + j;
                if (iListCheckBoard[iChecking] == 0)
                {
                    if (iListCheckBoard[iChecking + 1] != 0)
                    {
                        pushAble = true;
                    }
                }
                else if (iListCheckBoard[iChecking] == iListCheckBoard[iChecking + 1])
                {
                    pushAble = true;
                }
            }
        }
        return pushAble;
    }

    public void PushRight()
    {
        int iComparing; //正在受比對的格子索引值
        int iPolling; //輪尋格子的索引值
        bool pollFinish = false; //這一波輪尋是否已結束
        for (int i = 0; i < iLength; i++)
        {
            for (int j = 0; j < iLength; j++)
            {
                pollFinish = false;
                iComparing = ((i + 1) * iLength - 1) - j;
                iPolling = iComparing - 1;
                while (!pollFinish)
                {
                    if (iPolling < (i * iLength))//若輪尋格子超出了目前的這條行線
                    {
                        pollFinish = true;
                    }
                    else if (iListCheckBoard[iPolling] != 0)//若輪尋格子有東西(不為0)
                    {
                        if (iListCheckBoard[iComparing] == 0)//但受比對的格子是空的(0)的話，那就直接把數字搬過來
                        {
                            iListCheckBoard[iComparing] = iListCheckBoard[iPolling];
                            iListCheckBoard[iPolling] = 0;
                        }
                        else //其餘狀況，也就是當受比對的格子有東西(不為0)
                        {
                            if (iListCheckBoard[iComparing] == iListCheckBoard[iPolling])//若比對者與輪尋者皆同，兩者合併
                            {
                                iListCheckBoard[iComparing] += iListCheckBoard[iPolling];
                                this.iScore += iListCheckBoard[iComparing];
                                iListCheckBoard[iPolling] = 0;
                            }
                            else if (iListCheckBoard[iComparing - 1] == 0)//若比對者的隔壁沒東西(0)，那就把輪尋到的那個數字直接搬過來
                            {
                                iListCheckBoard[iComparing - 1] += iListCheckBoard[iPolling];
                                iListCheckBoard[iPolling] = 0;
                            }
                            pollFinish = true;
                        }
                    }
                    else//其餘狀況，也就是當輪尋格子沒東西(0)，且輪尋值也還未到底時
                    {
                        iPolling--;
                    }
                }
            }
        }
    }

    public bool PushAbleRight()
    {
        bool pushAble = false; //是否能推
        int iChecking;//檢查用的格子，從最開始(第0個)檢查到最後
        for (int i = 0; i < iLength && !pushAble; i++)
        {
            for (int j = 0; j < iLength - 1 && !pushAble; j++)
            {
                iChecking = ((i + 1) * iLength - 1) - j;
                if (iListCheckBoard[iChecking] == 0)
                {
                    if (iListCheckBoard[iChecking - 1] != 0)
                    {
                        pushAble = true;
                    }
                }
                else if (iListCheckBoard[iChecking] == iListCheckBoard[iChecking - 1])
                {
                    pushAble = true;
                }
            }
        }
        return pushAble;
    }

    public void PushUp()
    {
        int iComparing; //正在受比對的格子索引值
        int iPolling; //輪尋格子的索引值
        bool pollFinish = false; //這一波輪尋是否已結束
        for (int i = 0; i < iLength; i++)
        {
            for (int j = 0; j < iLength; j++)
            {
                pollFinish = false;
                iComparing = i + (j * iLength);
                iPolling = iComparing + iLength;
                while (!pollFinish)
                {
                    if (iPolling >= iLength * iLength)//若輪尋格子超出了目前的這條行線
                    {
                        pollFinish = true;
                    }
                    else if (iListCheckBoard[iPolling] != 0)//若輪尋格子有東西(不為0)
                    {
                        if (iListCheckBoard[iComparing] == 0)//但受比對的格子是空的(0)的話，那就直接把數字搬過來
                        {
                            iListCheckBoard[iComparing] = iListCheckBoard[iPolling];
                            iListCheckBoard[iPolling] = 0;
                        }
                        else //其餘狀況，也就是當受比對的格子有東西(不為0)
                        {
                            if (iListCheckBoard[iComparing] == iListCheckBoard[iPolling])//若比對者與輪尋者皆同，兩者合併
                            {
                                iListCheckBoard[iComparing] += iListCheckBoard[iPolling];
                                this.iScore += iListCheckBoard[iComparing];
                                iListCheckBoard[iPolling] = 0;
                            }
                            else if (iListCheckBoard[iComparing + iLength] == 0)//若比對者的隔壁沒東西(0)，那就把輪尋到的那個數字直接搬過來
                            {
                                iListCheckBoard[iComparing + iLength] += iListCheckBoard[iPolling];
                                iListCheckBoard[iPolling] = 0;
                            }
                            pollFinish = true;
                        }
                    }
                    else//其餘狀況，也就是當輪尋格子沒東西(0)，且輪尋值也還未到底時
                    {
                        iPolling += iLength;
                    }
                }
            }
        }
    }

    public bool PushAbleUp()
    {
        bool pushAble = false; //是否能推
        int iChecking;//檢查用的格子，從最開始(第0個)檢查到最後
        for (int i = 0; i < iLength && !pushAble; i++)
        {
            for (int j = 0; j < iLength - 1 && !pushAble; j++)
            {
                iChecking = i + (j * iLength);
                if (iListCheckBoard[iChecking] == 0)
                {
                    if (iListCheckBoard[iChecking + iLength] != 0)
                    {
                        pushAble = true;
                    }
                }
                else if (iListCheckBoard[iChecking] == iListCheckBoard[iChecking + iLength])
                {
                    pushAble = true;
                }
            }
        }
        return pushAble;
    }

    public void PushDown()
    {
        int iComparing; //正在受比對的格子索引值
        int iPolling; //輪尋格子的索引值
        bool pollFinish = false; //這一波輪尋是否已結束
        for (int i = 0; i < iLength; i++)
        {
            for (int j = 0; j < iLength; j++)
            {
                pollFinish = false;
                iComparing = i + ((iLength - 1) - j) * iLength;
                iPolling = iComparing - iLength;
                while (!pollFinish)
                {
                    if (iPolling < 0)//若輪尋格子超出了目前的這條行線
                    {
                        pollFinish = true;
                    }
                    else if (iListCheckBoard[iPolling] != 0)//若輪尋格子有東西(不為0)
                    {
                        if (iListCheckBoard[iComparing] == 0)//但受比對的格子是空的(0)的話，那就直接把數字搬過來
                        {
                            iListCheckBoard[iComparing] = iListCheckBoard[iPolling];
                            iListCheckBoard[iPolling] = 0;
                        }
                        else //其餘狀況，也就是當受比對的格子有東西(不為0)
                        {
                            if (iListCheckBoard[iComparing] == iListCheckBoard[iPolling])//若比對者與輪尋者皆同，兩者合併
                            {
                                iListCheckBoard[iComparing] += iListCheckBoard[iPolling];
                                this.iScore += iListCheckBoard[iComparing];
                                iListCheckBoard[iPolling] = 0;
                            }
                            else if (iListCheckBoard[iComparing - iLength] == 0)//若比對者的隔壁沒東西(0)，那就把輪尋到的那個數字直接搬過來
                            {
                                iListCheckBoard[iComparing - iLength] += iListCheckBoard[iPolling];
                                iListCheckBoard[iPolling] = 0;
                            }
                            pollFinish = true;
                        }
                    }
                    else//其餘狀況，也就是當輪尋格子沒東西(0)，且輪尋值也還未到底時
                    {
                        iPolling -= iLength;
                    }
                }
            }
        }
    }

    public bool PushAbleDown()
    {
        bool pushAble = false; //是否能推
        int iChecking;//檢查用的格子，從最開始(第0個)檢查到最後
        for (int i = 0; i < iLength && !pushAble; i++)
        {
            for (int j = 0; j < iLength - 1 && !pushAble; j++)
            {
                iChecking = i + ((iLength - 1) - j) * iLength;
                if (iListCheckBoard[iChecking] == 0)
                {
                    if (iListCheckBoard[iChecking - iLength] != 0)
                    {
                        pushAble = true;
                    }
                }
                else if (iListCheckBoard[iChecking] == iListCheckBoard[iChecking - iLength])
                {
                    pushAble = true;
                }
            }
        }
        return pushAble;
    }

}