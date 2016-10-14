using UnityEngine;
using System.Collections;

public class EnemyMoveTest : MonoBehaviour {
    [SerializeField]
    private float aroundPerSecond = 1.0f;
    private float aroundSpeed = 0.0f;
    private float euler = 0;
    //出現時のポジションを記憶
    private Vector3 startPos;
    //行動の半径
    [SerializeField]
    private float radius = 1.0f;

    [SerializeField]
    private MoveType moveType;
    [SerializeField]
    private RotateDirection rotDir;

    private enum MoveType
    {
        tate,
        yoko,
        en
    }
    private enum RotateDirection
    {
        right,
        left
    }


	// Use this for initialization
	void Start () {
        euler = Random.Range(0.0f, 360.0f);
        //一周にかける時間から速度を算出
        aroundSpeed = 360.0f / aroundPerSecond;
	}
    void OnEnable()
    {
        startPos = transform.position;
    }
	
	// Update is called once per frame
	void Update () {
        if (rotDir == RotateDirection.right)
            euler += aroundSpeed * Time.deltaTime;
        else euler -= aroundSpeed * Time.deltaTime;

        var circlePos = Vector3.zero;
        circlePos.x = Mathf.Sin(euler * Mathf.Deg2Rad) * radius;
        circlePos.y = Mathf.Cos(euler * Mathf.Deg2Rad) * radius;

        switch (moveType)
        {
            case MoveType.tate:
                circlePos.x = 0.0f;
                transform.position = startPos + circlePos;
                break;
            case MoveType.yoko:
                circlePos.y = 0.0f;
                transform.position = startPos + circlePos;
                break;
            case MoveType.en:
                transform.position = startPos + circlePos;
                break;
            default:

                break;
        }
	}
}
