using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameStateManager : MonoBehaviour {

    [Header("リザルト")]
    public Result result;
    [Header("パネルの拡大縮小させるマネージャー")]
    public PanelScalingManager panelScalingManager;

    private bool onceResult;

    public enum Status
    {
        Title,
        Game,
        Result,
        Ranking
    }

    [SerializeField,Header("現在のシーン")]
    private Status nowState;

    [SerializeField,Header("制限時間")]
    private float gameLimit;
    private float timer;

    //スコアにする場合廃止
    [SerializeField,Header("かぼちゃを倒した数")]
    private int pumpkinCount = 0;

    [SerializeField,Header("スタートのためのゲームオブジェクト")]
    private GameObject startButton;
    [SerializeField,Header("敵のプレハブ")]
    private GameObject enemy;
    [SerializeField,Header("最初に出す敵の数")]
    private int pumpNum;
    [SerializeField,Header("敵との距離")]
    private float radius = 3.0f;

    [SerializeField, Header("ゲーム開始前のカウントダウンテキスト")]
    private Text countDownText;
    [SerializeField, Header("ゲームの残り時間表示用テキスト")]
    private Text timeLimitText;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        switch (nowState)
        {
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

    void PumpkinGenerator()
    {
        //改善した
        for (int i = 0; i < pumpNum; i++)
        {
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
    IEnumerator GameStartCountDown(int cnt)
    {
        float oneSec = 0.0f;
        string cntStr = cnt.ToString();
        while (cnt > 0) 
        {
            oneSec += Time.deltaTime;
            //点滅させるために1秒のうち0.5秒は表示しない
            if (oneSec > 0.5f)
            {
                cntStr = "";
            }
            countDownText.text = cntStr;
            if (oneSec > 1.0f)
            {
                oneSec = 0.0f;
                cnt--;
                cntStr = cnt.ToString();
            }
            yield return null;
        }
        //スタートを0.5秒表示した後ゲーム開始
        countDownText.text = "START";
        yield return new WaitForSeconds(0.5f);
        countDownText.text = "";
        nowState = Status.Game;
        PumpkinGenerator();
    }
    //ゲーム終了後TIMEUPを表示
    IEnumerator GameFinishText()
    {
        countDownText.text = "TIME UP";
        //表示秒数をとりあえず1秒に
        float waitTime = 1.0f;
        yield return new WaitForSeconds(waitTime);
        countDownText.text = "";
        timeLimitText.text = "";
        nowState = Status.Result;
        result.score = pumpkinCount;
        startButton.SetActive(true);
        result.callViewResult();
    }

    void GameUpdate()
    {

        timer -= Time.deltaTime;
        if (timer < 0.0f && !onceResult)
        {
            onceResult = true;
            timeLimitText.text = "0";
            GoResult();
            return;
        }
        int cnt = (int)timer + 1;
        timeLimitText.text = cnt.ToString();
    }

    public void KillPumpkin(int cnt)
    {
        pumpkinCount += cnt;
    }

    public void GoGame()
    {
        if(nowState == Status.Result) {
            result.state = Result.State.PANEL_REDUCTION;
        } else {
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
    public void GoTitle()
    {
        nowState = Status.Title;
        startButton.SetActive(true);
    }
    public void GoResult()
    {
        //出現しているかぼちゃを消す
        EnemyManager.Instance.ActiveAllFalse();

        StartCoroutine(GameFinishText());
    }
    public void GoRanking()
    {
        nowState = Status.Ranking;
    }


    public void GoGameInitialize() {
        onceResult = false;
        timer = gameLimit;
        pumpkinCount = 0;
        startButton.SetActive(false);
        //開始前のカウントダウン
        //引数はカウントダウンの時間
        //数値は未定
        StartCoroutine(GameStartCountDown(3));
    }

}
