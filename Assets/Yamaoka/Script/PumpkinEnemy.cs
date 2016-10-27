using UnityEngine;
using System.Collections;
using UnityEngine.Events;

//コメント不足
public class PumpkinEnemy : MonoBehaviour {
//全動きのパターン共通
    //

    //[HideInInspector]
    //public UnityEvent CollisionPotato;

    private GameStateManager gameState;
    private MeshFilter mesh;
    private MeshCollider meshCollider;
    private int pumpScore;

    [SerializeField,Header("プレイヤーの位置用")]
    private Transform playerTrans;
    //プレイヤーからの距離と方向と高さ
    //生成時にランダムに決定
    private float distance;
    private float direction;
    private float height;

    //芋が当たったときのノックバック
    private Vector3 deadVec;
    private bool isDead = false;
    private float deadTimer = 0.0f;
    [SerializeField,Header("芋が当たってから消えるまでの時間")]
    private float vanishTime;
    [SerializeField, Header("芋が当たってからかぼちゃに発生する重力")]
    private float gravity;

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

        mesh = transform.GetChild(0).GetComponent<MeshFilter>();
        meshCollider = GetComponent<MeshCollider>();

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
        transform.position += vec * Time.deltaTime;

        if (deadTimer>vanishTime)
        {
            //煙出現

            gameObject.SetActive(false);
            deadTimer = 0.0f;
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
        angle = 0;

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
        angle = 0;

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
        angle = 0;

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
        angle = 0;

        var pos = playerTrans.position;
        pos += Quaternion.Euler(0.0f, direction, 0.0f) * Vector3.forward * distance;
        pos.y = height;
        startPos = pos;

        transform.position = startPos;
        transform.LookAt(playerTrans);

        RotDirSelect();
        CircleMove();
    }
    public void SetMeshAndScore(Mesh _mesh, int _score)
    {
        mesh.mesh = _mesh;
        meshCollider.sharedMesh = _mesh;
        pumpScore = _score;
        //transform.position = Vector3.down * 1000;
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
        if (col.gameObject.tag == "Potato")
        {
            //CollisionPotato.Invoke();
            gameState.KillPumpkin(pumpScore);
            gameObject.SetActive(false);
        }

    }
}
