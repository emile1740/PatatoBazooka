using UnityEngine;
using System.Collections;
using UnityEngine.Events;

//コメント不足
public class PumpkinEnemy : MonoBehaviour {
    //全動きのパターン共通
    //
    [HideInInspector]
    public UnityEvent CollisionPotato;
    [SerializeField,Header("プレイヤーの位置用")]
    private Transform playerTrans;
    //プレイヤーからの距離と方向
    //生成時にランダムに決定
    private float distance=15.0f;
    private float direction;

    //ここまで共通

    [SerializeField,Header("1周にかかる時間")]
    private float aroundPerSecond = 1.0f;
    //360/aroundPerSecondでスピードを設定
    private float aroundSpeed = 0.0f;
    private float euler = 0;
    //出現時のポジションを記憶
    private Vector3 startPos;
    //行動の半径
    [SerializeField,Header("行動の半径")]
    private float radius = 1.0f;

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
        var temp = GameObject.FindObjectOfType<GameStateManager>();
        playerTrans = GameObject.FindGameObjectWithTag("Player").transform;
        CollisionPotato.AddListener(temp.KillPumpkin);
        startPos = transform.position;
        //一周にかける時間から速度を算出
        aroundSpeed = 360.0f / aroundPerSecond;
        gameObject.SetActive(false);
	}

    void OnEnable()
    {
        startPos = transform.position;

        transform.LookAt(playerTrans);

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
	
	// Update is called once per frame
	void Update () {
        transform.LookAt(playerTrans);

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

    void RandomMove()
    {

    }

    void AroundMove()
    {
        if (isTurnRight)
            euler += aroundSpeed * Time.deltaTime;
        else euler -= aroundSpeed * Time.deltaTime;

        var circlePos = Vector3.zero;
        circlePos.x = Mathf.Cos(euler * Mathf.Deg2Rad) * radius; 
        circlePos.z = Mathf.Sin(euler * Mathf.Deg2Rad) * radius;

        transform.position = playerTrans.position + circlePos;
    }

    void AroundWaveMove()
    {

    }

    //縦の動き
    void VerticalMove()
    {
        if (isTurnRight)
            euler += aroundSpeed * Time.deltaTime;
        else euler -= aroundSpeed * Time.deltaTime;

        var circlePos = Vector3.zero;
        circlePos.y = Mathf.Cos(euler * Mathf.Deg2Rad) * radius;

        transform.position = startPos + circlePos;
    }
    //横の動き
    void LateralMove()
    {
        if (isTurnRight)
            euler += aroundSpeed * Time.deltaTime;
        else euler -= aroundSpeed * Time.deltaTime;

        var circlePos = Vector3.zero;
        circlePos.x = Mathf.Sin(euler * Mathf.Deg2Rad) * radius;

        transform.position = startPos + circlePos;
    }
    //円の動き
    void CircleMove()
    {
        if (isTurnRight)
            euler += aroundSpeed * Time.deltaTime;
        else euler -= aroundSpeed * Time.deltaTime;

        var circlePos = Vector3.zero;
        circlePos.x = Mathf.Sin(euler * Mathf.Deg2Rad) * radius;
        circlePos.y = Mathf.Cos(euler * Mathf.Deg2Rad) * radius;
        circlePos = transform.rotation * circlePos;
        transform.position = startPos + circlePos;
    }


    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Potato")
        {
            CollisionPotato.Invoke();
            gameObject.SetActive(false);
        }

    }
}
