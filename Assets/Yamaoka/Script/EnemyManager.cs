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

    [SerializeField, Header("敵との距離幅")]
    private float minDistance;
    [SerializeField]
    private float maxDistance;

    [SerializeField, Header("敵の高さの幅")]
    private float minHeight;
    [SerializeField]
    private float maxHeight;

    [SerializeField, Header("敵の周回の時間幅")]
    private float minAroundTime;
    [SerializeField]
    private float maxAroundTime;

    [SerializeField, Header("敵の周回時の波幅")]
    private float minWaveRadius;
    [SerializeField]
    private float maxWaveRadius;
    [SerializeField, Header("敵の周回時の波の時間幅")]
    private float minWaveTime;
    [SerializeField]
    private float maxWaveTime;

    [SerializeField, Header("敵の回転の幅")]
    private float minSpinRadius;
    [SerializeField]
    private float maxSpinRadius;
    [SerializeField, Header("敵の回転の時間幅")]
    private float minSpinTime;
    [SerializeField]
    private float maxSpinTime;

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
                TypeSelect(temp);
                temp.gameObject.SetActive(true);
                return temp;
            }
        }
        return null;
    }
    //この2つは多分使わない
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

    void TypeSelect(PumpkinEnemy enem)
    {
        //ランダムは選ばれない：完成してないため
        int type = Random.Range(1, 6);

        float dis = Random.Range(minDistance, maxDistance);
        float dir = Random.Range(0.0f, 360.0f);
        float high = Random.Range(minHeight, maxHeight);

        float aroundTime = Random.Range(minAroundTime, maxAroundTime);

        float spinRad = Random.Range(minSpinRadius, maxSpinRadius);
        float spinTime = Random.Range(minSpinTime, maxSpinTime);

        switch (type)
        {
            case 0:
                Debug.Log("0");
                enem.Random();
                break;
            case 1:
                Debug.Log("1");
                enem.Around(dis,dir,high,aroundTime);
                break;
            case 2:
                Debug.Log("2");
                float waveRad = Random.Range(minWaveRadius, maxWaveRadius);
                float waveTime = Random.Range(minWaveTime, maxWaveTime);
                enem.AroundWave(dis, dir, high, aroundTime, waveRad, waveTime);
                break;
            case 3:
                Debug.Log("3");
                enem.Vertical(dis,dir,high,spinTime,spinRad);
                break;
            case 4:
                Debug.Log("4");
                enem.Lateral(dis,dir,high,spinTime,spinRad);
                break;
            case 5:
                Debug.Log("5");
                enem.Circle(dis,dir,high,spinTime,spinRad);
                break;
            default:
                Debug.Log("Miss");
                break;

        }

    }
}
