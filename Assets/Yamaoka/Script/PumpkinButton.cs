using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class PumpkinButton : MonoBehaviour {
    [SerializeField]
    private GameStateManager.Status destination;
    [HideInInspector]
    public UnityEvent CollisionPotato;
    private bool onceCollision = false;

    private float cooltimer;
    [SerializeField]
    private const float COOL_TIMER = 1.0f;

    private Vector3 defaultPosition;
    [Header("縦揺れする幅")]
    public float swayWidth;
    private DIRECTION direction;
    private enum DIRECTION {
        UP = 1,
        DOWN = -1
    }

    void Start()
    {
        var temp = GameObject.FindObjectOfType<GameStateManager>();
        switch (destination)
        {
            case GameStateManager.Status.Title:
                CollisionPotato.AddListener(temp.GoTitle);
                direction = DIRECTION.DOWN;
                break;
            case GameStateManager.Status.Game:
                CollisionPotato.AddListener(temp.GoGame);
                direction = DIRECTION.UP;
                break;
            case GameStateManager.Status.Result:
                CollisionPotato.AddListener(temp.GoResult);
                break;
            case GameStateManager.Status.Ranking:
                CollisionPotato.AddListener(temp.GoRanking);
                direction = DIRECTION.DOWN;
                break;

            default:
                break;
        }

        defaultPosition = transform.localPosition;
    }

    void OnEnable()
    {
        onceCollision = false;
    }

    void Update() {
        if (onceCollision) {
            cooltimer += Time.deltaTime;
            if (cooltimer > COOL_TIMER) {
                onceCollision = false;
            }
        }

        //縦揺れの処理
        //現状リザルト画面からタイトルに戻るときに
        //右のカボチャが一瞬カクつくよ（かぼちゃのSetActiveが切り替わった瞬間）
        var pos = defaultPosition;
        var adjust = defaultPosition.y + ((swayWidth / 2) * (int)direction * -1);
        pos.y = adjust + Mathf.PingPong(Time.time, swayWidth) * (int)direction;
        transform.localPosition = pos;
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Potato" && !onceCollision) {
            onceCollision = true;
            cooltimer = 0.0f;
            CollisionPotato.Invoke();
        }
    }

}
