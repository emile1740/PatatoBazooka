using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyManager : SingletonMonoBehaviour<EnemyManager>
{
    [SerializeField]
    private GameObject pumpkin;
    [SerializeField, Header("ゲーム中のかぼちゃの上限")]
    private int maxPumpkin;
    private List<PumpkinEnemy> pumpkins = new List<PumpkinEnemy>();

    void Awake()
    {
        if (this != Instance)
        {
            Destroy(this);
            return;
        }

        for (int i = 0; i < maxPumpkin; i++)
        {
            var temp = (GameObject)Instantiate(pumpkin, Vector3.zero, Quaternion.identity);
            pumpkins.Add(temp.GetComponent<PumpkinEnemy>());
            temp.transform.parent = transform;
        }
    }

    //ゲーム終了時に消す
    public void ActiveAllFalse()
    {
        foreach (PumpkinEnemy temp in pumpkins)
        {
            temp.gameObject.SetActive(false);
        }
    }

    //Instantiateのように使える
    public PumpkinEnemy GetEnemy()
    {
        foreach (PumpkinEnemy temp in pumpkins)
        {
            if (!temp.gameObject.activeInHierarchy)
            {
                temp.gameObject.SetActive(true);
                return temp;
            }
        }
        return null;
    }
    public PumpkinEnemy GetEnemy(Vector3 pos)
    {
        foreach (PumpkinEnemy temp in pumpkins)
        {
            if (!temp.gameObject.activeInHierarchy)
            {
                temp.transform.position = pos;
                temp.gameObject.SetActive(true);
                
                return temp;
            }
        }
        return null;
    }
    public PumpkinEnemy GetEnemy(Vector3 pos, Quaternion rot)
    {
        foreach (PumpkinEnemy temp in pumpkins)
        {
            if (!temp.gameObject.activeInHierarchy)
            {
                temp.transform.position = pos;
                temp.transform.rotation = rot;
                temp.gameObject.SetActive(true);

                return temp;
            }
        }
        return null;
    }
}
