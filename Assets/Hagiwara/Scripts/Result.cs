using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;

/// <summary>
/// リザルト画面を処理
/// </summary>
public class Result : MonoBehaviour {

    private const string RANKING_FILE_PATH = "Assets/Resources/ranking.csv";

    [Header("結果表示用パネル")]
    public RectTransform resultPanel;
    private float expandTimer;
    [Header("結果表示用パネルを拡大させる時間")]
    public float MAX_EXPAND_TIME;


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

    private RectTransform storeRect;

    [Header("ランキングデータのスクロールスピード")]
    public float scrollSpeed;

    [Header("今回のランキングNo")]
    public int rankingNo;

    [Header("ランキングに入った番号のオブジェクト")]
    public RectTransform inRankingObjectRect;

    [Header("ランキングデータ")]
    public int[] rankingData = new int[100];

    [Header("遷移ステータス")]
    public State state;
    public enum State {
        START,
        PANEL_EXPAND,
        NOT_IN_RANKING,
        PLAYSE,
        SCROLL,
        END
    }

    [Header("ランクイン時のフレーム")]
    public Image countFrame;
    private float countFrameTime;
    [Header("ランクイン時のフレームの点滅間隔")]
    public float falshInterval;

    // Use this for initialization
    void Start() {
        storeRect = rankingDataStore.GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update() {

        //とりあえず、エンターキーを押したら、パネルを拡大表示する
        if (state == State.START && Input.GetKeyDown(KeyCode.Return)) {
            state = State.PANEL_EXPAND;

        } else if (state == State.NOT_IN_RANKING) {
            setRankingData();

            if (rankingNo != rankingData.Length) {
                setRankingImage();
                state = State.PLAYSE;
            }
        }

        stateProcessing();
    }

    private void stateProcessing() {

        switch (state) {

            case State.PANEL_EXPAND:
                //パネルの拡大表示
                //var scale = resultPanel.localScale;
                expandTimer += Time.deltaTime;
                var scale = Mathf.Lerp(0f,1f, expandTimer / MAX_EXPAND_TIME);
                resultPanel.localScale = new Vector3(scale, scale, scale);
                if (scale == 1.0f) state = State.NOT_IN_RANKING;
                break;
            case State.NOT_IN_RANKING:
                //ランキング内に入っていない場合
                break;

            case State.PLAYSE:
                //ランキング内に入っている場合、ドラムロール音を再生する
                AudioManager.Instance.PlaySE("se_drumroll");
                state = State.SCROLL;
                break;

            case State.SCROLL:
                //ランキング内に入っている場合、スクロール処理
                scrollRanking();
                break;
            case State.END:
                //ランキング内に入っている場合、スクロール処理後にフレームを点滅させる
                flashCountFrame();
                break;
        }
    }

/// <summary>
/// ランキングデータを設定
/// </summary>
private void setRankingData() {

        //ランキングデータの読み込み
        rankingNo = rankingData.Length;

        //ReadCsv();

        var fr = new FileReader(this);
        fr.ReadCsv();

        //Debug.Log("ランキングデータの読み込み後 Ranking No " + rankingNo);

        //今回のスコアがランクインされていたら、順位を変更して書き換える
        if (rankingNo != rankingData.Length) {
            //WriteCsv();
            var fw = new FileWriter(this);
            fw.WriteCsv();
        }

    }

    ///// <summary>
    ///// ランキングデータの読み込み
    ///// </summary>
    //private void ReadCsv() {

    //    try {
    //        ReadCSVFile();
    //    } catch (System.Exception e) {
    //        // ファイルを開くのに失敗したとき
    //        System.Console.WriteLine(e.Message);
    //        Debug.Log("failed");

    //        WriteCSVFile();
    //        ReadCSVFile();
    //    }
    //}

    ///// <summary>
    ///// CSVファイルから読み込み
    ///// </summary>
    //private void ReadCSVFile() {
    //    int index = 0;
    //    StreamReader reader = new StreamReader(RANKING_FILE_PATH);

    //    // ストリームの末尾まで繰り返す
    //    while (reader.Peek() > -1) {

    //        // ファイルから一行読み込む
    //        var lineValue = int.Parse(reader.ReadLine());

    //        //読み込んだスコアよりも今回のスコアのほうが大きい場合、
    //        //あとでそのランキングの場所に挿入するために、ランキングNoを保存しておく
    //        Debug.Log("rankingNo " + rankingNo);
    //        Debug.Log("lineValue " + lineValue + " score " + score);
    //        if (rankingNo == rankingData.Length && lineValue < score) rankingNo = index;

    //        rankingData[index] = lineValue;
    //        index++;
    //    }

    //    Debug.Log("success");
    //    reader.Close();
    //}

    ///// <summary>
    ///// ランキングデータの書き込み
    ///// </summary>
    //private void WriteCsv() {

    //    //今回ランクインされる順位以降の順位を１つ下げて、該当の順位に今回のスコアを入れる
    //    for (int index = rankingData.Length - 2; index >= rankingNo; index--) {
    //        if (rankingData[index + 1] != rankingData[index]) rankingData[index + 1] = rankingData[index];
    //    }
    //    rankingData[rankingNo] = score;

    //    //書き込み処理
    //    try {
    //        WriteCSVFile();

    //    } catch (System.Exception e) {
    //        // ファイルを開くのに失敗したときエラーメッセージを表示
    //        System.Console.WriteLine(e.Message);
    //        Debug.Log("Write failed");
    //    }
    //}

    ///// <summary>
    ///// CSVファイルへの書き込み
    ///// </summary>
    //private void WriteCSVFile() {
    //    StreamWriter writer = new StreamWriter(RANKING_FILE_PATH, false);

    //    for (int i = 0; i < rankingData.Length; i++) writer.WriteLine(rankingData[i]);

    //    writer.Flush();
    //    writer.Close();
    //    Debug.Log("Write success");
    //}

    /// <summary>
    /// ランキング順位の生成
    /// </summary>
    private void setRankingImage() {

        //１００件分のランキングデータを上方向に生成していく
        for (int index = 0; index < rankingData.Length; index++) {
            var pos = rankingImagePrefab.transform.position;
            pos.y += (rankingImagePrefab.GetComponent<RectTransform>().sizeDelta.y + rankingImage_AdjustPosition) * index;
            var rankingImageObj = (GameObject)Instantiate(rankingImagePrefab,pos,Quaternion.identity);

            rankingImageObj.transform.SetParent(rankingDataStore.transform,false);
            rankingImageObj.name = rankingImagePrefab.name + "_" + index;

            //ランキング順位の表示
            var rank = rankingData.Length - index;

            //ランキングスコア画像の設定
            setRankingScoreImage(rankingImageObj, rankingData[rank - 1]);

            //ランキング順位画像の設定
            setRankingCountImage(index, rankingImageObj ,rank);
        }

        inRankingObjectRect = rankingDataStore.transform.GetChild((rankingData.Length - 1) - rankingNo).GetComponent<RectTransform>();
        //changeMyScoreImageColor(inRankingObjectRect.FindChild("RankImage"));
        //changeMyScoreImageColor(inRankingObjectRect.FindChild("ScoreImage"));
        countFrame.transform.SetParent(inRankingObjectRect);
        countFrame.transform.localPosition = Vector3.zero;
    }

    /// <summary>
    /// ランキングスコア画像の設定
    /// </summary>
    /// <param name="rankingImageObj"></param>
    /// <param name="scoreTmp"></param>
    private void setRankingScoreImage(GameObject rankingImageObj, int scoreTmp) {

        var scoreImage = rankingImageObj.transform.FindChild("ScoreImage");

        for (int i = scoreImage.childCount - 1; i >= 0; i--) {

            changeCountImage(scoreImage.GetChild(i).GetComponent<Image>(), scoreTmp % 10);
            scoreTmp /= 10;

            //５桁分の数字の画像を変更する前に、計算するスコアが０になったら残りの画像を０にしてループを抜ける
            if (scoreTmp == 0 && i > 0) {
                while (i > 0) changeCountImage(scoreImage.GetChild(--i).GetComponent<Image>(), 0);
                break;
            }
        }
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

    ///// <summary>
    ///// ランキングに入った場合、自分のスコアが分かりやすいように色を変更する
    ///// </summary>
    ///// <param name="tf"></param>
    //private void changeMyScoreImageColor(Transform tf) {
    //    foreach(Transform t in tf) t.GetComponent<Image>().color = new Color(1,0,0);
    //}

    /// <summary>
    /// ランキングデータのスクロール処理
    /// </summary>
    private void scrollRanking() {

        ////格納フォルダの座標を動かす
        //var pos = storeRect.transform.position;
        //pos.y -= Time.deltaTime * scrollSpeed;
        //var nextPos = pos;


        ////対象のオブジェクト
        //if(inRankingObjectRect.localPosition = )







        ////考え方を変える必要がある
        //for (int index=0; index<rankingData.Length; index++) {
        //    var child = rankingDataStore.transform.GetChild(index);
        //    var pos = child.localPosition;
        //    pos.y -= Time.deltaTime * scrollSpeed;
        //    child.localPosition = pos;

            //    Transform stopTargetObject = null;
            //    if (rankingNo >= 3 && rankingNo <= (rankingData.Length - 1) - 2) {
            //        if (inRankingObjectRect == child) {
            //            stopTargetObject = child;
            //        }
            //    } else {

            //        if (rankingNo < 3) {
            //            //ランキングに入った順位が１位か２位の場合
            //            stopTargetObject = rankingDataStore.transform.GetChild((rankingData.Length - 1) - 2);
            //        } else if (rankingNo > (rankingData.Length - 1) - 2) {
            //            //ランキングに入った順位が９９位か１００位の場合
            //            stopTargetObject = rankingDataStore.transform.GetChild(2);
            //        }

            //    }
            //    if (stopTargetObject != null && stopTargetObject.localPosition.y <= 0.0f) {
            //        var pos_2 = stopTargetObject.transform.localPosition;
            //        pos_2.y = 0.0f;
            //        stopTargetObject.transform.localPosition = pos_2;

            //        AudioManager.Instance.StopSE();
            //        AudioManager.Instance.PlaySE("se_cymbal");
            //        state = State.END;
            //        break;
            //    }
            //}
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
