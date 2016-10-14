using UnityEngine;
using System.Collections;

public class EnemyMoveTest2 : MonoBehaviour {
    [SerializeField]
    private Transform playerTrans;
    [SerializeField]
    private float aroundPerSecond = 1.0f;
    private float aroundSpeed = 0.0f;
    private float euler = 0;

    //行動の半径
    [SerializeField]
    private float radius = 1.0f;




    // Use this for initialization
    void Start()
    {

    }
    void OnEnable()
    {
       
    }

    // Update is called once per frame
    void Update()
    {

        var circlePos = Vector3.zero;
        circlePos.x = Mathf.Sin(euler * Mathf.Deg2Rad) * radius;
        circlePos.y = Mathf.Cos(euler * Mathf.Deg2Rad) * radius;

        //switch (moveType)
        //{
        //    case MoveType.tate:
        //        circlePos.x = 0.0f;
        //        transform.position = startPos + circlePos;
        //        break;
        //    case MoveType.yoko:
        //        circlePos.y = 0.0f;
        //        transform.position = startPos + circlePos;
        //        break;
        //    case MoveType.en:
        //        transform.position = startPos + circlePos;
        //        break;
        //    default:

        //        break;
        //}
    }
}
