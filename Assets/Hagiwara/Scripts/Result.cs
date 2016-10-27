﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

/// <summary>
/// リザルト画面を処理
/// </summary>
public class Result : MonoBehaviour {

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
    public GameObject countFrame;
    private GameObject myScoreText;

    private Dictionary<GameObject, Vector3> rankingDataDic;

    /// <summary>
    /// 値の初期化
    /// </summary>
    public void initialize() {
        state = State.START;
        rankingNo = rankingData.Length;

        countFrame.SetActive(true);
        myScoreImageParent.SetActive(true);
        myScoreText.SetActive(true);

        //各ランキングデータのオブジェクトを初期位置に戻す
        foreach (Transform child in rankingDataStore.transform) {
            child.localPosition = rankingDataDic[child.gameObject];
        }

        Debug.Log("result - initialize");
    }

    /// <summary>
    /// タイトル画面からランキング画面に遷移してきた時に呼ばれる
    /// </summary>
    public void initialize_ComeTitle() {
        state = State.START;
        rankingNo = 0;
        countFrame.SetActive(false);
        myScoreImageParent.SetActive(false);

        myScoreText.SetActive(false);

        float firstPos = 200.0f;
        int upperLevel = 5;
        Vector3 pos = Vector3.zero;

        //上位５件分のデータの座標を設定する
        for (int i=0;i< upperLevel; i++) {
            var child = rankingDataStore.transform.GetChild(99 - i);
            setRankingScoreImage(child.gameObject, rankingData[i]);

            pos.y = firstPos - i * (child.GetComponent<RectTransform>().sizeDelta.y + rankingImage_AdjustPosition);
            child.localPosition = pos;
        }
    }

    // Use this for initialization
    void Start() {

        //ランキングデータの読み込み
        var fr = new FileReader(this);
        fr.ReadCsv();

        //for(int i=0;i<rankingDataStore ;int++)
        rankingDataDic = new Dictionary<GameObject, Vector3>();

        //各ランキングデータのオブジェクトの初期位置を保存しておく
        foreach (Transform child in rankingDataStore.transform) {
            rankingDataDic.Add(child.gameObject, child.localPosition);
        }

        myScoreText = resultPanel.FindChild("TEXTUIPanel/MyScoreText").gameObject;

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

        rankingDataInsertScore();
        setMyScoreData();
        
        if(!panelScalingManager.panelDic.ContainsKey(resultPanel))
            panelScalingManager.addPanelDictionary(resultPanel,this);

        state = State.PANEL_EXPAND;
    }


    /// <summary>
    /// タイトルからランキングを選んだ時に呼ばれる
    /// </summary>
    public void callViewResult_ComeTitle() {
        initialize_ComeTitle();

        if (!panelScalingManager.panelDic.ContainsKey(resultPanel))
            panelScalingManager.addPanelDictionary(resultPanel, this);

        state = State.PANEL_EXPAND;
    }

    // Update is called once per frame
    void Update() {

        //デバッグ用、エンターキーを押したら、パネルを拡大表示する
        if (state == State.START && Input.GetKeyDown(KeyCode.Return)) {
            //callViewResult();
            callViewResult_ComeTitle();
        }

        if (state == State.NOT_IN_RANKING) {

            //ランキングに変更があった場合、
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
                panelScalingManager.setPanelScaling(resultPanel,0.0f, 1.0f);
                break;

            case State.PANEL_REDUCTION:
                //パネルの縮小表示
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
        }
    }

    /// <summary>
    /// 拡大縮小演出が終了したら呼ばれる
    /// </summary>
    public void PanelScalingEnd() {
        //拡大縮小演出が終了したら前のステータスの状態によって次の遷移先を設定
        if (state == State.PANEL_EXPAND) {

            //ゲーム画面から呼ばれた場合はランキングデータの設定に移行
            state = State.NOT_IN_RANKING;

            //タイトル画面から呼ばれた場合は
            state = State.END;

        } else if (state == State.PANEL_REDUCTION) {
            state = State.END;
            gameStateManager.GoGameInitialize();
        }
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
                for (int j = rankingData.Length - 1; j > rankingNo; j--) {
                    rankingData[j] = rankingData[j-1];
                }
                rankingData[rankingNo] = score;
            }
        }
    }

    /// <summary>
    /// ランキング順位の生成
    /// </summary>
    private void setRankingImage() {

        //１００件分のランキングデータを順番に参照
        for (int i = 0; i < rankingData.Length; i++) {
            var rankingImageObj = rankingDataStore.transform.GetChild(i).gameObject;

            //ランキング順位の表示
            var rank = rankingData.Length - i;

            //ランキングスコア画像の設定
            setRankingScoreImage(rankingImageObj, rankingData[rank - 1]);
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

        //各ランキングデータのオブジェクトに終了座標を設定する
        //終了座標 = 現在の場所 - 目的地までの移動量
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
}
