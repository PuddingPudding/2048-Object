using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool g_uniqueInstance = new ObjectPool();
    [SerializeField] private GameObject m_gameObjBlockPrefab; //每個格子的遊戲物件原型
    [SerializeField] private List<GameObject> m_listBlockPrefab;

    private ObjectPool() //先將自己的建構子宣告私有化，避免別人亂生孩子
    {
        m_listBlockPrefab = new List<GameObject>();
    }
    public ObjectPool Instance
    {
        get
        {
            if(g_uniqueInstance == null)
            {
                g_uniqueInstance = new ObjectPool();
            }
            return g_uniqueInstance;
        }
    }

    public GameObject GetBlockPrefab()
    {
        GameObject gameObjBlock;
        if (m_listBlockPrefab.Count > 0)
        {
            gameObjBlock = m_listBlockPrefab[m_listBlockPrefab.Count - 1];
            m_listBlockPrefab.RemoveAt(m_listBlockPrefab.Count - 1);
        }
        else
        {
            gameObjBlock = Instantiate(m_gameObjBlockPrefab);
        }
        gameObjBlock.SetActive(true);
        return gameObjBlock;
    }
    public void BackToBlockPool(GameObject _gameObjBlock)
    {
        m_listBlockPrefab.Add(_gameObjBlock);
        _gameObjBlock.SetActive(false);
    }
    private void Awake()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
}
