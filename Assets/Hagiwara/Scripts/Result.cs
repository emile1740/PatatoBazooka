using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

/// <summary>
/// リザルト画面を処理
/// </summary>
public class Result : MonoBehaviour {

    private const string RANKING_FILE_PATH = "Assets/Resources/ranking.csv";

    [Header("ゲーム全体のステータスマネージャー")]
    public GameStateManager gameStateManager;

    [Header("パネルの拡大縮小させるマネージャー")]
    public PanelScalingManager panelScalingManager;

    [Header("結果表示用パネル")]
    public RectTransform resultPanel;

    [Header("共通用オブジェクト")]
    public Common common;

    [Header("ランキングデータのプレハブ")]
    public GameObject rankingImagePrefab;

    [Header("ランキングデータを生成する補正ポジション")]
    public float rankingImage_AdjustPosition;

    [Header("ランキングデータを格納するオブジェクト")]
    public GameObject rankingDataStore;

    [Header("今回のスコア")]
    public int score;
    [Header("今回のスコア用の親オブジェクト")]
    public GameObject myScoreImageParent;

    [Header("ランキングデータのスクロールスピード")]
    public float scrollSpeed;

    [Header("今回のランキングNo")]
    public int rankingNo;

    [Header("ランキングに入った番号のオブジェクト")]
    public RectTransform inRankingObjectRect;

    [Header("ランキングデータ")]
    public int[] rankingData = new int[100];

    //スクロールの終了座標
    private Vector3[] scrollTargetPosition = new Vector3[100];

    [Header("遷移ステータス")]
    public State state;
    public enum State {
        START,
        PANEL_EXPAND,
        PANEL_REDUCTION,
        NOT_IN_RANKING,
        SCROLL,
        END
    }

    [Header("ランクイン時のフレーム")]
    public Image countFrame;
    private float countFrameTime;
    [Header("ランクイン時のフレームの点滅間隔")]
    public float falshInterval;

    private Dictionary<GameObject, RectTransform> rankingDataDic;

    /// <summary>
    /// 値の初期化
    /// </summary>
    public void initialize() {
        state = State.START;
        rankingNo = rankingData.Length;
        Debug.Log("result - initialize");
    }

    // Use this for initialization
    void Start() {

        //ランキングデータの読み込み
        var fr = new FileReader(this);
        fr.ReadCsv();

    }

    /// <summary>
    /// アプリケーション終了時にランキングデータに変更があればCSVファイルに書き込む
    /// </summary>
    void OnApplicationQuit() {
        //今回のスコアがランクインされていたら、順位を変更して書き換える
        if (rankingNo != rankingData.Length) {
            var fw = new FileWriter(this);
            fw.WriteCsv();
        }
    }


    /// <summary>
    /// タイムアップでゲームが終了した時に呼ばれる
    /// </summary>
    public void callViewResult() {
        initialize();

        //resultPanel.gameObject.SetActive(true);

        rankingDataInsertScore();
        setMyScoreData();
        
        if(!panelScalingManager.panelDic.ContainsKey(resultPanel))
            panelScalingManager.addPanelDictionary(resultPanel,this);

        state = State.PANEL_EXPAND;
    }

    // Update is called once per frame
    void Update() {

        //とりあえず、エンターキーを押したら、パネルを拡大表示する
        if (state == State.START && Input.GetKeyDown(KeyCode.Return)) {
            callViewResult();

        } else if (state == State.NOT_IN_RANKING) {
            //setRankingData();

            //ランキングに変更があった場合
            //ランクインした順位が中央に来るように移動量を計算し、
            //その移動量分、他の順位も移動させる
            if (rankingNo != rankingData.Length) {

                setRankingImage();

                //ランクインしたオブジェクトを設定
                var rank = (rankingData.Length - 1) - rankingNo;
                inRankingObjectRect = rankingDataStore.transform.GetChild(rank).GetComponent<RectTransform>();

                //ランクインしたスコア用のフレームを設定
                countFrame.transform.SetParent(inRankingObjectRect);
                countFrame.transform.localPosition = Vector3.zero;

                setScrollTargetPosition();

                //ドラムロール音を再生する
                AudioManager.Instance.PlaySE("se_drumroll");
                state = State.SCROLL;
            }
        }

        stateProcessing();
    }

    /// <summary>
    /// 遷移ステータスに対応する処理を行う
    /// </summary>
    private void stateProcessing() {

        switch (state) {

            case State.PANEL_EXPAND:
                //パネルの拡大表示
                //resultPanelScaling(0.0f, 1.0f);
                panelScalingManager.setPanelScaling(resultPanel,0.0f, 1.0f);
                break;

            case State.PANEL_REDUCTION:
                //パネルの縮小表示
                //resultPanelScaling(1.0f, 0.0f);
                panelScalingManager.setPanelScaling(resultPanel, 1.0f, 0.0f);
                break;

            case State.NOT_IN_RANKING:
                //ランキング内に入っていない場合
                state = State.END;
                break;

            case State.SCROLL:
                //ランキング内に入っている場合、スクロール処理
                scrollRanking();
                break;

            case State.END:
                //ランキング内に入っている場合、スクロール処理後にフレームを点滅させる
                //flashCountFrame();
                break;
        }
    }

    /// <summary>
    /// 拡大縮小演出が終了したら呼ばれる
    /// </summary>
    public void PanelScalingEnd() {
        //拡大縮小演出が終了したら前のステータスの状態によって次の遷移先を設定
        if (state == State.PANEL_EXPAND) {
            state = State.NOT_IN_RANKING;
        } else if (state == State.PANEL_REDUCTION) {
            state = State.END;
            gameStateManager.GoGameInitialize();
        }
        panelScalingManager.initialize();
    }

    /// <summary>
    /// 今回のスコアの画像を設定
    /// </summary>
    private void setMyScoreData() {
        scoreConversion(myScoreImageParent.transform,score);
    }

    /// <summary>
    /// スコアに対応した画像に変換する
    /// </summary>
    /// <param name="tf"></param>
    /// <param name="scoreTmp"></param>
    private void scoreConversion(Transform tf,int scoreTmp) {

        for (int i = tf.childCount - 1; i >= 0; i--) {

            changeCountImage(tf.GetChild(i).GetComponent<Image>(), scoreTmp % 10);
            scoreTmp /= 10;

            //５桁分の数字の画像を変更する前に、計算するスコアが０になったら残りの画像を０にしてループを抜ける
            if (scoreTmp == 0 && i > 0) {
                while (i > 0) changeCountImage(tf.GetChild(--i).GetComponent<Image>(), 0);
                break;
            }
        }
    }

    /// <summary>
    /// ランキングデータの配列に今回のスコアを追加する
    /// </summary>
    private void rankingDataInsertScore() {

        //読み込んだスコアよりも今回のスコアのほうが大きい場合、
        //あとでそのランキングの場所に挿入するために、ランキングNoを保存しておく      
        for (int i=0;i<rankingData.Length && rankingNo == rankingData.Length; i++) {

            if (rankingData[i] < score) {
                rankingNo = i;
                for (int j = rankingNo; j < rankingData.Length - 1; j++) {
                    rankingData[j + 1] = rankingData[j];
                }
                rankingData[rankingNo] = score;
            }
        }
    }


    ///// <summary>
    ///// ランキングデータを設定
    ///// </summary>
    //private void setRankingData() {

    //    //ランキングの読み込みタイミングはゲーム起動時に変更する必要がある

    //    //ランキングデータの読み込み
    //    rankingNo = rankingData.Length;

    //    var fr = new FileReader(this);
    //    fr.ReadCsv();

    //    ランキングの書き込みタイミングはゲーム終了時に変更する必要がある

    //    //今回のスコアがランクインされていたら、順位を変更して書き換える
    //    if (rankingNo != rankingData.Length) {
    //        var fw = new FileWriter(this);
    //        fw.WriteCsv();
    //    }

    //}

    /// <summary>
    /// ランキング順位の生成
    /// </summary>
    private void setRankingImage() {

        //１００件分のランキングデータを順番に参照
        for (int index = 0; index < rankingData.Length; index++) {
            //var pos = rankingImagePrefab.transform.position;
            //pos.y += (rankingImagePrefab.GetComponent<RectTransform>().sizeDelta.y + rankingImage_AdjustPosition) * index;
            //var rankingImageObj = (GameObject)Instantiate(rankingImagePrefab,pos,Quaternion.identity);

            //rankingImageObj.transform.SetParent(rankingDataStore.transform,false);
            //rankingImageObj.name = rankingImagePrefab.name + "_" + index;

            var rankingImageObj = rankingDataStore.transform.GetChild(index).gameObject;

            //ランキング順位の表示
            var rank = rankingData.Length - index;

            //ランキングスコア画像の設定
            setRankingScoreImage(rankingImageObj, rankingData[rank - 1]);

            //ランキング順位画像の設定
            setRankingCountImage(index, rankingImageObj ,rank);
        }
    }

    /// <summary>
    /// ランキングスコア画像の設定
    /// </summary>
    /// <param name="rankingImageObj"></param>
    /// <param name="scoreTmp"></param>
    private void setRankingScoreImage(GameObject rankingImageObj, int scoreTmp) {
        var scoreImage = rankingImageObj.transform.FindChild("ScoreImage");
        scoreConversion(scoreImage, scoreTmp);
    }

    /// <summary>
    /// ランキング順位画像の設定
    /// </summary>
    /// <param name="index"></param>
    /// <param name="rankingImageObj"></param>
    /// <param name="rank"></param>
    private void setRankingCountImage(int index,GameObject rankingImageObj, int rank) {

        var rankImage = rankingImageObj.transform.FindChild("RankImage");
        var rankTmp = rank - 1;

        //１の位は必ず表示
        viewRankingImage(rankImage.GetChild(2).GetComponent<Image>(), rank % 10);
        rank /= 10;

        //ランキング１００位の場合、１０の位、１００の位を表示
        if (rankTmp == (rankingData.Length - 1)) {
            viewRankingImage(rankImage.GetChild(0).GetComponent<Image>(), 1);
            viewRankingImage(rankImage.GetChild(1).GetComponent<Image>(), 0);
        } else {
            //ランキング１０位以上の場合、１０の位を表示
            if (rankTmp >= 9) {
                viewRankingImage(rankImage.GetChild(1).GetComponent<Image>(), rank % 10);
            }
        }
    }

    /// <summary>
    /// ランキング順位を表示
    /// </summary>
    /// <param name="image"></param>
    /// <param name="countIndex"></param>
    private void viewRankingImage(Image image, int countIndex) {
        image.enabled = true;
        changeCountImage(image, countIndex);
    }

    /// <summary>
    /// 数字の画像を変更
    /// </summary>
    /// <param name="image"></param>
    /// <param name="countIndex"></param>
    private void changeCountImage(Image image, int countIndex) {
        image.sprite = common.count[countIndex];
    }

    /// <summary>
    /// スクロールの終了座標を設定
    /// </summary>
    private void setScrollTargetPosition() {

        Vector3 distance;
        if (rankingNo < 2) {
            //rankingNo < 2 ランキング順位２位以上の場合、３位と同じ移動量を設定する
            distance = rankingDataStore.transform.GetChild((rankingData.Length - 1) - 2).localPosition - countFrame.transform.localPosition;

        } else if (rankingNo > (rankingData.Length - 1) - 2) {
            //rankingNo > (rankingData.Length - 1) - 2 (97) ランキング順位９９位以上の場合、９８位と同じ移動量を設定する
            distance = rankingDataStore.transform.GetChild(2).localPosition - countFrame.transform.localPosition;
        } else {
            distance = inRankingObjectRect.localPosition - countFrame.transform.localPosition;
        }

        for (int i = 0; i < rankingData.Length; i++) {
            scrollTargetPosition[i] = rankingDataStore.transform.GetChild(i).localPosition - distance;
        }
    }

    /// <summary>
    /// ランキングオブジェクトのスクロール処理
    /// </summary>
    private void scrollRanking() {

        //各オブジェクトを終了座標まで移動させる
        for (int i=0;i<rankingData.Length ;i++) {
            var child = rankingDataStore.transform.GetChild(i);
            child.localPosition = Vector3.MoveTowards(child.localPosition,scrollTargetPosition[i],Time.deltaTime * scrollSpeed);
        }
        //各オブジェクトが終了座標まで移動したかをチェックし、移動していたらスクロール終了処理
        if (checkScrollTarget()) {
            AudioManager.Instance.StopSE();
            AudioManager.Instance.PlaySE("se_cymbal");
            state = State.END;
        }
    }

    /// <summary>
    /// 全てのランキングオブジェクトが終了座標まで移動したどうかをチェック（全て終了していたらtrueを取得）
    /// </summary>
    /// <returns></returns>
    private bool checkScrollTarget() {

        for (int i = 0; i < rankingData.Length; i++) {
            var child = rankingDataStore.transform.GetChild(i);
            if (!Mathf.Approximately(child.localPosition.y, scrollTargetPosition[i].y)) {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// ランクインしたスコアのフレームを点滅させる
    /// </summary>
    private void flashCountFrame() {
        
        countFrameTime += Time.deltaTime;
        if((int)((countFrameTime + falshInterval) % 2) == 0) {
            countFrame.enabled = true;
        }else {
            countFrame.enabled = false;
        }
    }
}
