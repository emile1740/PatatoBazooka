using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyManager : SingletonMonoBehaviour<EnemyManager>
{
    [SerializeField, Header("かぼちゃの点数(大きい順)")]
    private int[] pumpkinPoint;
    [SerializeField, Header("かぼちゃの種類(大きい順)")]
    private Mesh[] pumpkinMeshes;
    [SerializeField, Header("煙の種類(かぼちゃの色順)")]
    private GameObject[] pumpkinParticles;
    [SerializeField, Header("")]
    private GameObject pumpkin;
    [SerializeField, Header("ゲーム中のかぼちゃの上限")]
    private int maxPumpkin;
    private List<PumpkinEnemy> pumpkins = new List<PumpkinEnemy>();

    [SerializeField, Header("敵との距離幅")]
    private float minDistance;
    [SerializeField]
    private float maxDistance;
    private float typeDis = 0.0f;

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

    //１ゲーム中に出現した数を種類ごとにカウント
    private int[] typeCounter = new int[4]{1,1,1,1};
    //カウントから算出した比率
    private int[] typeRaito = new int[4];
    //現在のかぼちゃの数
    [SerializeField,Header("みるだけ")]
    private int nowPumpkinCount = 0;

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
        typeDis = (maxDistance - minDistance) / 4.0f;
    }

    //void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.T))
    //    {
    //        for (int i = 0; i < typeCounter.Length; i++)
    //        {
    //            Debug.Log((i+1).ToString()+"番目は"+typeCounter[i].ToString()+"体");
    //        }
    //    }
    //}

    public void PumpkinCountDecrement()
    {
        nowPumpkinCount--;
        if (nowPumpkinCount < 0)
            nowPumpkinCount = 0;
    }

    //ゲーム開始時に芋のカウントを初期化
    public void ResetCount()
    {
        for (int i = 0; i < typeCounter.Length; i++)
        {
            typeCounter[i] = 1;
        }
        nowPumpkinCount = 0;
    }

    //ゲーム終了時に消す
    public void ActiveAllFalse()
    {
        foreach (PumpkinEnemy temp in pumpkins)
        {
            temp.gameObject.SetActive(false);
        }
    }

    //ランダムで出す
    public void RandomJudge()
    {
        //とりあえず今の敵の数と最大出せる数の割合を確率
        //max100,now20なら80パーセント
        float _current = nowPumpkinCount * 1.0f;
        float _parent = maxPumpkin * 1.0f;
        float per = _current / _parent;
        if (Random.value > per)
            GetEnemy();

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

    public void TypeSelect(PumpkinEnemy enem)
    {
        //仕様変更
        //ここで種類を先に決める
        int type=-1;
        //カウントの合計から、各種類のカウントを引くことで、丁度いい比率になる
        int sum = 0;
        foreach (int a in typeCounter)
            sum += a;
        for (int i = 0; i < typeCounter.Length; i++)
        {
            typeRaito[i] = sum - typeCounter[i];
        }
        //0から合計までのランダムな数字を求め、それを比率の各数値の範囲に入るかを判断
        sum = 0;
        foreach (int a in typeRaito)
            sum += a;
        int ran = Random.Range(0, sum);
        sum = 0;
        for (int i = 0; i < typeRaito.Length; i++)
        {
            sum += typeRaito[i];
            if (ran < sum)
            {
                type = i;
                i += typeRaito.Length;
            }
        }
        //通ったら予想外の動き
        if (type < 0)
        {
            Debug.Log("タイプ設定エラー");
            type = 3;
        }


        float dis = minDistance;
        //得点が多いタイプほど距離を伸ばす
        for (int i = type; i > 0; i--)
            dis += typeDis;
        dis += Random.Range(0.0f, typeDis);



        //ランダムは選ばれない：完成してないため
        int move = Random.Range(1, 6);

        //float dis = Random.Range(minDistance, maxDistance);
        float dir = Random.Range(0.0f, 360.0f);
        float high = Random.Range(minHeight, maxHeight);

        float aroundTime = Random.Range(minAroundTime, maxAroundTime);

        float spinRad = Random.Range(minSpinRadius, maxSpinRadius);
        float spinTime = Random.Range(minSpinTime, maxSpinTime);

        switch (move)
        {
            case 0:
                Debug.Log("0");
                enem.RandoM();
                break;
            case 1:
                enem.Around(dis,dir,high,aroundTime);
                break;
            case 2:
                float waveRad = Random.Range(minWaveRadius, maxWaveRadius);
                float waveTime = Random.Range(minWaveTime, maxWaveTime);
                enem.AroundWave(dis, dir, high, aroundTime, waveRad, waveTime);
                break;
            case 3:
                enem.Vertical(dis,dir,high,spinTime,spinRad);
                break;
            case 4:
                enem.Lateral(dis,dir,high,spinTime,spinRad);
                break;
            case 5:
                enem.Circle(dis,dir,high,spinTime,spinRad);
                break;
            default:
                Debug.Log("EnemyTypeMissing");
                break;

        }


        
        //メッシュと点数を決める
        //上に書くとやばそうなので全て決めた後に書く
        enem.SetMeshAndScore(pumpkinMeshes[type], pumpkinPoint[type], pumpkinParticles[type]);
        //出現した種類が出にくくなるように比率調整
        typeCounter[type]++;
        //現在いる数としてカウント
        nowPumpkinCount++;
        if (nowPumpkinCount > maxPumpkin)
            nowPumpkinCount = maxPumpkin;

        //下は過去のメッシュ設定
        ////type = Random.Range(0, 4);

        ////自分との距離で決めるように
        //for (int i = 0; i < 4; i++)
        //{
        //    if (dis < minDistance + typeDis * (i + 1) || i == 3) 
        //    {
        //        enem.SetMeshAndScore(pumpkinMeshes[i], pumpkinPoint[i],pumpkinParticles[i]);
        //        i += 4;
        //    }
        //}

    }
}
