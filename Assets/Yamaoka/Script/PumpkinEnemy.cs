using UnityEngine;
using System.Collections;
using UnityEngine.Events;

//コメント不足
public class PumpkinEnemy : MonoBehaviour {
    //全動きのパターン共通
    // 
    private AudioSource hitAudio;
    //[HideInInspector]
    //public UnityEvent CollisionPotato;

    private GameStateManager gameState;
    private MeshFilter mesh;
    private MeshCollider meshCollider;
    private int pumpScore;
    private int scoreNum;

    [SerializeField,Header("プレイヤーの位置用")]
    private Transform playerTrans;
    //プレイヤーからの距離と方向と高さ
    //生成時にランダムに決定
    private float distance;
    private float direction;
    private float height;

    //芋が当たったときのノックバック
    [SerializeField]
    private Vector3 deadVec;
    private bool isDead = false;
    private float deadTimer = 0.0f;
    [SerializeField,Header("芋が当たってから消えるまでの時間")]
    private float vanishTime;
    [SerializeField, Header("芋が当たってからかぼちゃに発生する重力")]
    private float gravity;

    //子のかぼちゃを回転させるために使う
    private Transform modelTrans;
    private Quaternion modelStartLocalRot;

    //芋ヒット時にエフェクトを発生
    [SerializeField, Header("ヒットエフェクトのパーティクル")]
    private GameObject HitEffectParticle;

    //消滅時に煙を発生
    [SerializeField,Header("煙のパーティクル")]
    private GameObject vanishSmokeParticle;
    [SerializeField,Header("煙をどのくらい前に出すか")]
    private float forwardDistance;

//ここまで共通
    
//周の動き共通
    [SerializeField, Header("1周にかかる時間")]
    private float aroundPerSecond = 1.0f;
    //360/aroundPerSecondでスピードを設定
    private float aroundSpeed = 0.0f;
//ここまで共通

//円の動き共通
    private float angle = 0;
    //行動の半径
    [SerializeField, Header("行動の半径")]
    private float radius = 1.0f;
    [SerializeField, Header("1回転にかかる時間")]
    private float rotatePerSecond = 1.0f;
    private float rotateSpeed;
//ここまで共通

    
    
    //軸となるポジション
    private Vector3 startPos;
    
    

    [SerializeField]
    private MoveType moveType;
    [SerializeField, Header("右回りならtrue")]
    private bool isTurnRight;
    private enum MoveType
    {
        random,
        around,
        aroundWave,
        vertical,
        lateral,
        circle
    }

    
	// Use this for initialization
	void Start () {
        gameState = GameObject.FindObjectOfType<GameStateManager>();
        playerTrans = GameObject.FindGameObjectWithTag("Player").transform;
        //CollisionPotato.AddListener(temp.KillPumpkin);
        //一周にかける時間から速度を算出
        aroundSpeed = 360.0f / aroundPerSecond;

        //消滅時にモデルを回転させるため
        modelTrans = transform.GetChild(0);
        modelStartLocalRot = modelTrans.localRotation;

        //再利用時に種類を変えるため
        mesh = modelTrans.GetComponent<MeshFilter>();
        meshCollider = GetComponent<MeshCollider>();

        hitAudio = GetComponent<AudioSource>();

        gameObject.SetActive(false);
	}

	// Update is called once per frame
	void Update () {
        if (isDead)
        {
            DeadMove();
            return;
        }

        switch (moveType)
        {
            case MoveType.random:
                RandomMove();
                break;
            case MoveType.around:
                AroundMove();
                break;
            case MoveType.aroundWave:
                AroundWaveMove();
                break;
            case MoveType.vertical:
                VerticalMove();
                break;
            case MoveType.lateral:
                LateralMove();
                break;
            case MoveType.circle:
                CircleMove();
                break;
            default:

                break;
        }
	}

    //芋が当たったあとの動き
    void DeadMove()
    {
        deadTimer += Time.deltaTime;
        var vec = deadVec;
        vec.y -= gravity * deadTimer;
        transform.position += transform.rotation * vec * Time.deltaTime;

        modelTrans.Rotate(Vector3.back * 90.0f * Time.deltaTime);

        if (deadTimer>vanishTime)
        {
            //煙出現
            var pos = transform.position;
            pos += transform.rotation * Vector3.forward * forwardDistance;
            ObjectPool.Instance.GetGameObject(vanishSmokeParticle, pos, transform.rotation);

            gameObject.SetActive(false);
            deadTimer = 0.0f;
            isDead = false;
        }
    }

    //ランダムな動き
    void RandomMove()
    {

    }
    //周の動き
    void AroundMove()
    {
        if (isTurnRight)
            direction += aroundSpeed * Time.deltaTime;
        else direction -= aroundSpeed * Time.deltaTime;

        var circlePos = Vector3.zero;
        circlePos.x = Mathf.Cos(direction * Mathf.Deg2Rad) * distance;
        circlePos.z = Mathf.Sin(direction * Mathf.Deg2Rad) * distance;

        transform.position = startPos + circlePos;
        transform.LookAt(playerTrans);
    }
    //波の動き
    void AroundWaveMove()
    {
        if (isTurnRight)
            direction += aroundSpeed * Time.deltaTime;
        else direction -= aroundSpeed * Time.deltaTime;

        var circlePos = Vector3.zero;
        circlePos.x = Mathf.Cos(direction * Mathf.Deg2Rad) * distance;
        circlePos.z = Mathf.Sin(direction * Mathf.Deg2Rad) * distance;

        angle += aroundSpeed * Time.deltaTime;
        circlePos.y = Mathf.Sin(angle * Mathf.Deg2Rad) * radius;

        transform.position = startPos + circlePos;
        transform.LookAt(playerTrans);
    }
    //縦の動き
    void VerticalMove()
    {
        if (isTurnRight)
            angle += aroundSpeed * Time.deltaTime;
        else angle -= aroundSpeed * Time.deltaTime;

        var circlePos = Vector3.zero;
        circlePos.y = Mathf.Sin(angle * Mathf.Deg2Rad) * radius;
        
        transform.position = startPos + circlePos;
    }
    //横の動き
    void LateralMove()
    {
        if (isTurnRight)
            angle += aroundSpeed * Time.deltaTime;
        else angle -= aroundSpeed * Time.deltaTime;

        var circlePos = Vector3.zero;
        circlePos.x = Mathf.Cos(angle * Mathf.Deg2Rad) * radius;
        circlePos = transform.rotation * circlePos;

        transform.position = startPos + circlePos;
    }
    //円の動き
    void CircleMove()
    {
        if (isTurnRight)
            angle += aroundSpeed * Time.deltaTime;
        else angle -= aroundSpeed * Time.deltaTime;

        var circlePos = Vector3.zero;
        circlePos.x = Mathf.Sin(angle * Mathf.Deg2Rad) * radius;
        circlePos = transform.rotation * circlePos;
        circlePos.y = Mathf.Cos(angle * Mathf.Deg2Rad) * radius;

        transform.position = startPos + circlePos;
    }

    //Randomクラスと区別
    public void RandoM()
    {

    }
    public void Around(float dis,float dir,float high,float aroundTime)
    {
        moveType = MoveType.around;

        distance = dis;
        direction = dir;
        height = high;
        aroundPerSecond = aroundTime;
        aroundSpeed = 360.0f / aroundPerSecond;

        startPos = playerTrans.position;
        startPos.y += height;

        RotDirSelect();
        AroundMove();
    }
    public void AroundWave(float dis, float dir, float high, float aroundTime, float waveRad, float waveTime)
    {
        moveType = MoveType.aroundWave;

        distance = dis;
        direction = dir;
        height = high;
        aroundPerSecond = aroundTime;
        radius = waveRad;
        rotatePerSecond = waveTime;
        rotateSpeed = 360.0f / rotatePerSecond;
        angle = Random.Range(0.0f, 360.0f);

        startPos = playerTrans.position;
        startPos.y += height;

        RotDirSelect();
        AroundWaveMove();
    }
    public void Vertical(float dis, float dir, float high, float rotateTime, float rad)
    {
        moveType = MoveType.vertical;

        distance = dis;
        direction = dir;
        height = high;
        rotatePerSecond = rotateTime;
        rotateSpeed = 360.0f / rotatePerSecond;
        radius = rad;
        rotatePerSecond = rotateTime;
        rotateSpeed = 360.0f / rotatePerSecond;
        angle = Random.Range(0.0f, 360.0f);

        var pos = playerTrans.position;
        pos += Quaternion.Euler(0.0f, direction, 0.0f) * Vector3.forward * distance;
        pos.y = height;
        startPos = pos;

        transform.position = startPos;
        transform.LookAt(playerTrans);

        VerticalMove();
    }
    public void Lateral(float dis, float dir, float high, float rotateTime, float rad)
    {
        moveType = MoveType.lateral;

        distance = dis;
        direction = dir;
        height = high;
        rotatePerSecond = rotateTime;
        rotateSpeed = 360.0f / rotatePerSecond;
        radius = rad;
        rotatePerSecond = rotateTime;
        rotateSpeed = 360.0f / rotatePerSecond;
        angle = Random.Range(0.0f, 360.0f);

        var pos = playerTrans.position;
        pos += Quaternion.Euler(0.0f, direction, 0.0f) * Vector3.forward * distance;
        pos.y = height;
        startPos = pos;

        transform.position = startPos;
        transform.LookAt(playerTrans);

        LateralMove();
    }
    public void Circle(float dis, float dir, float high, float rotateTime, float rad)
    {
        moveType = MoveType.circle;

        distance = dis;
        direction = dir;
        height = high;
        rotatePerSecond = rotateTime;
        rotateSpeed = 360.0f / rotatePerSecond;
        radius = rad;
        rotatePerSecond = rotateTime;
        rotateSpeed = 360.0f / rotatePerSecond;
        angle = Random.Range(0.0f, 360.0f);

        var pos = playerTrans.position;
        pos += Quaternion.Euler(0.0f, direction, 0.0f) * Vector3.forward * distance;
        pos.y = height;
        startPos = pos;

        transform.position = startPos;
        transform.LookAt(playerTrans);

        RotDirSelect();
        CircleMove();
    }
    public void SetMeshAndScore(int _scoreNum, Mesh _mesh, int _score,GameObject particle)
    {
        scoreNum = _scoreNum;
        mesh.mesh = _mesh;
        meshCollider.sharedMesh = _mesh;
        pumpScore = _score;
        vanishSmokeParticle = particle;
        //transform.position = Vector3.down * 1000;


        //かぼちゃの角度を直す
        modelTrans.localRotation = modelStartLocalRot;

    }
    //時計回りか反時計回りを決める
    void RotDirSelect()
    {
        isTurnRight = false;
        int rand = Random.Range(0, 2);
        if (rand > 0)
        {
            isTurnRight = true;
        }
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Potato" && !isDead)
        {
            //CollisionPotato.Invoke();
            ObjectPool.Instance.GetGameObject(HitEffectParticle, col.contacts[0].point, transform.rotation);
            gameState.KillPumpkin(scoreNum,pumpScore);
            EnemyManager.Instance.PumpkinCountDecrement();
            hitAudio.Play();
            isDead = true;
            //gameObject.SetActive(false);
        }

    }
}
