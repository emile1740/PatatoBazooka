using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameStateManager : MonoBehaviour {

    [Header("リザルト")]
    public Result result;
    [Header("パネルの拡大縮小させるマネージャー")]
    public PanelScalingManager panelScalingManager;

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
    [SerializeField, Header("敵との距離")]
    private float radius = 3.0f;

    [SerializeField, Header("ゲーム開始前のカウントダウンテキスト")]
    private Text countDownText;
    [SerializeField, Header("ゲームの残り時間表示用テキスト")]
    private Text timeLimitText;
    [SerializeField, Header("スコア表示用テキスト")]
    private Text scoreText;

    [SerializeField, Header("開始、終了時の煙パーティクル")]
    private ParticleSystem smokeParticle;

    void Start() {
        AudioManager.Instance.PlayBGM("bgm_Title",true);
    }

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

        timer -= Time.deltaTime;
        if (timer < 0.0f && !onceResult) {
            onceResult = true;
            timeLimitText.text = "0";
            GoResult();
            return;
        }

        int cnt = (int)timer + 1;
        timeLimitText.text = cnt.ToString();
    }

    public void KillPumpkin(int cnt) {
        pumpkinCount += cnt;
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
            result.callViewResult_ComeTitle();
        } else {
            nowState = Status.Title;
            result.state = Result.State.PANEL_REDUCTION;
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

    }

    public void GoGameInitialize() {
        onceResult = false;

        timer = gameLimit;
        pumpkinCount = 0;

        result.resetExplotion();

        //ゲームスタート用のかぼちゃとランキング表示用のかぼちゃを非表示
        //タイトルへ戻る用のかぼちゃも非表示
        startButton.SetActive(false);
        rankingButton.SetActive(false);
        titleButton.SetActive(false);


        AudioManager.Instance.StopBGM();

        //開始前のカウントダウン
        //引数はカウントダウンの時間
        //数値は未定
        StartCoroutine(GameStartCountDown(3));
    }
}