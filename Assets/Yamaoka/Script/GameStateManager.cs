using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameStateManager : MonoBehaviour {

    [Header("共通オブジェクト")]
    public Common common;

    [Header("リザルト")]
    public Result result;
    [Header("加算スコア")]
    public AddPumpkinScore addPumpkinScore;

    [Header("パネルの拡大縮小させるマネージャー")]
    public PanelScalingManager panelScalingManager;

    [Header("タイトルロゴ")]
    public GameObject titleLogo;
    
    private bool onceResult;
    public enum Status {
        Title,
        Game,
        Result,
        Ranking
    }

    [Header("現在のシーン")]
    public Status nowState;

    [SerializeField, Header("制限時間")]
    private float gameLimit;
    private float timer;

    ////スコアにする場合廃止
    //スコアとして流用
    [SerializeField, Header("現在のスコア")]
    private int pumpkinCount = 0;

    [Header("スタートのためのゲームオブジェクト")]
    public GameObject startButton;
    [Header("ランキングのためのゲームオブジェクト")]
    public GameObject rankingButton;
    [Header("タイトル戻るためのゲームオブジェクト")]
    public GameObject titleButton;

    [SerializeField, Header("敵のプレハブ")]
    private GameObject enemy;
    [SerializeField, Header("最初に出す敵の数")]
    private int pumpNum;
    [SerializeField, Header("敵を出すか判定する間隔"), Range(0.1f, 2.0f)]
    private float pumpInterval = 1.0f;
    private float pumpTimer = 0.0f;

    [SerializeField, Header("敵との距離")]
    private float radius = 3.0f;

    [SerializeField, Header("ゲーム開始前のカウントダウンテキスト")]
    private Text countDownText;
    [SerializeField, Header("ゲームの残り時間表示用テキスト")]
    private Text timeLimitText;
    [SerializeField, Header("スコア表示用テキスト")]
    private Text scoreText;

    [SerializeField, Header("スコア表示オブジェクト")]
    private GameObject scoreObject;

    [SerializeField, Header("タイムリミットオブジェクト")]
    private GameObject timeLimitObject;
    [SerializeField, Header("タイムリミット用イメージ")]
    private Image[] timeLimitImage;

    [SerializeField, Header("開始、終了時の煙パーティクル")]
    private ParticleSystem smokeParticle;

    void Start() {
        AudioManager.Instance.PlayBGM("bgm_Title", true);
        countDownText.text = "";
        timeLimitText.text = "";
        scoreText.text = "";
    }

    [Header("TOP5画像切り替え用")]
    public SpriteRenderer top5_ButtonImage;
    public Sprite[] top5_ButtonImageList;

    // Update is called once per frame
    void Update() {
        switch (nowState) {
            case Status.Title:

                break;
            case Status.Game:
                GameUpdate();
                break;
            case Status.Result:
                break;
            case Status.Ranking:

                break;

            default:
                break;
        }

    }

    void PumpkinGenerator() {
        EnemyManager.Instance.ResetCount();
        //改善した
        for (int i = 0; i < pumpNum; i++) {
            EnemyManager.Instance.GetEnemy();
        }

        ////確認用
        //Transform playerTrans = GameObject.FindObjectOfType<Shoot>().transform;
        //float unitAngle = 360.0f / pumpNum ;
        //float angle = 30.0f;
        //for (int i = 0; i < pumpNum; i++)
        //{
        //    var pos = playerTrans.position + Quaternion.Euler(0.0f,angle,0.0f) * Vector3.forward * radius;
        //    EnemyManager.Instance.GetEnemy(pos);
        //    angle += 60;
        //}
    }

    //ゲーム開始前のカウントダウン
    //引数の秒数カウント
    IEnumerator GameStartCountDown(int cnt) {
        float oneSec = 0.0f;
        string cntStr = cnt.ToString();
        AudioManager.Instance.PlaySE("CountDown_01");
        while (cnt > 0) {
            oneSec += Time.deltaTime;
            //点滅させるために1秒のうち0.5秒は表示しない
            if (oneSec > 0.5f) {
                cntStr = "";
            }
            countDownText.text = cntStr;
            if (oneSec > 1.0f) {
                oneSec = 0.0f;
                cnt--;
                cntStr = cnt.ToString();
            }
            yield return null;
        }
        //スタートを0.5秒表示した後ゲーム開始
        countDownText.text = "START";
        smokeParticle.Play();
        yield return new WaitForSeconds(0.5f);
        countDownText.text = "";

        scoreObject.SetActive(true);
        timeLimitObject.SetActive(true);

        nowState = Status.Game;

        AudioManager.Instance.PlayBGM("bgm_Game_2",true);

        PumpkinGenerator();
    }
    //ゲーム終了後TIMEUPを表示
    IEnumerator GameFinishText() {
        countDownText.text = "TIME UP";
        smokeParticle.Play();
        AudioManager.Instance.PlaySE("TimeUp_01");
        //表示秒数をとりあえず1秒に
        float waitTime = 1.0f;
        yield return new WaitForSeconds(waitTime);
        countDownText.text = "";
        timeLimitText.text = "";
        scoreText.text = "";

        scoreObject.SetActive(false);
        timeLimitObject.SetActive(false);
        timeLimitImage[0].sprite = common.count[3];
        timeLimitImage[1].sprite = common.count[0];

        nowState = Status.Result;
        result.score = pumpkinCount;

        //ゲームスタート用のかぼちゃとタイトルに戻る用のかぼちゃを表示
        //startButton.SetActive(true);
        //titleButton.SetActive(true);

        result.callViewResult();
    }

    void GameUpdate() {

        scoreText.text = pumpkinCount.ToString();

        if (onceResult)
            return;

        pumpTimer += Time.deltaTime;
        if (pumpTimer > pumpInterval)
        {
            pumpTimer -= pumpInterval;
            EnemyManager.Instance.RandomJudge();
        }

        timer -= Time.deltaTime;
        if (timer < 0.0f && !onceResult) {
            onceResult = true;
            timeLimitText.text = "0";

            timeLimitImage[1].sprite = common.count[0];
            GoResult();
            return;
        }

        int cnt = (int)timer + 1;
        timeLimitText.text = cnt.ToString();

        timeLimitImage[0].sprite = common.count[cnt / 10];
        timeLimitImage[1].sprite = common.count[cnt % 10];
    }

    public void KillPumpkin(int scoreNum,int cnt) {
        pumpkinCount += cnt;
        addPumpkinScore.GenerateScoreImage(scoreNum);
        addPumpkinScore.setTargetScoreImage(pumpkinCount);
    }

    public void GoGame() {

        Debug.Log("GoGame");

        AudioManager.Instance.StopSE("se_drumroll");
        AudioManager.Instance.StopSE("se_cymbal");

        if (nowState == Status.Result || nowState == Status.Ranking) {
            result.state = Result.State.PANEL_REDUCTION;
            Debug.Log("now State " + nowState);
        }else {
            GoGameInitialize();
        }
        //timer = gameLimit;
        //pumpkinCount = 0;
        //startButton.SetActive(false);
        ////開始前のカウントダウン
        ////引数はカウントダウンの時間
        ////数値は未定
        //StartCoroutine(GameStartCountDown(3));
    }
    public void GoTitle() {
        Debug.Log("GoTitle");

        nowState = Status.Title;
        result.state = Result.State.PANEL_REDUCTION;

        AudioManager.Instance.PlayBGM("bgm_Title", true);
    }
    public void GoResult() {
        Debug.Log("GoResult");

        //出現しているかぼちゃを消す
        EnemyManager.Instance.ActiveAllFalse();

        AudioManager.Instance.StopBGM();

        StartCoroutine(GameFinishText());
    }
    public void GoRanking() {

        Debug.Log("GoRanking");

        if (nowState != Status.Ranking) {
            nowState = Status.Ranking;
            titleLogo.SetActive(false);
            result.callViewResult_ComeTitle();
            top5_ButtonImage.sprite = top5_ButtonImageList[1];
        } else {
            nowState = Status.Title;
            result.state = Result.State.PANEL_REDUCTION;
            top5_ButtonImage.sprite = top5_ButtonImageList[0];
        }
    }

    public void GoTitleInitialize() {
        result.resetExplotion();
        //result.isExplosionGenerate = false;

        AudioManager.Instance.StopSE("se_drumroll");
        AudioManager.Instance.StopSE("se_cymbal");

        //タイトルに戻る用のかぼちゃを非表示
        //ゲームスタート用のかぼちゃとランキング表示用のかぼちゃを表示
        titleButton.SetActive(false);

        startButton.SetActive(true);
        rankingButton.SetActive(true);

        titleLogo.SetActive(true);

        //ボタンが切り替わった瞬間にカクつかないように、座標を調整
        rankingButton.transform.localPosition = titleButton.transform.localPosition;

    }

    public void GoGameInitialize() {
        onceResult = false;

        pumpTimer = 0.0f;
        timer = gameLimit;
        pumpkinCount = 0;

        result.resetExplotion();

        //ゲームスタート用のかぼちゃとランキング表示用のかぼちゃを非表示
        //タイトルへ戻る用のかぼちゃも非表示
        startButton.SetActive(false);
        rankingButton.SetActive(false);
        titleButton.SetActive(false);

        titleLogo.SetActive(false);

        AudioManager.Instance.StopBGM();

        //開始前のカウントダウン
        //引数はカウントダウンの時間
        //数値は未定
        StartCoroutine(GameStartCountDown(3));
    }
}