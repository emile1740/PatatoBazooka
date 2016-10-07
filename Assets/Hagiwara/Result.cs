using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;

/// <summary>
/// リザルト画面を処理
/// </summary>
public class Result : MonoBehaviour {

    private const string RANKING_FILE_PATH = "Assets/Resources/ranking.csv";

    [Header("ランキングデータのプレハブ")]
    public GameObject rankingImagePrefab;

    [Header("ランキングデータを生成する補正ポジション")]
    public float rankingImage_AdjustPosition;

    [Header("ランキングデータを格納するオブジェクト")]
    public GameObject rankingDataStore;
    
    [Header("今回のスコア")]
    public int score;

    private bool isRankingSetting;
    private bool isInRanking;
    private RectTransform storeRect;

    [Header("ランキングデータのスクロールスピード")]
    public float scrollSpeed;

    [Header("今回のランキングNo")]
    public int rankingNo;

    private RectTransform inRankingObjectRect;

    [Header("ランキングデータ")]
    public int[] rankingData = new int[100];

    // Use this for initialization
    void Start() {
        storeRect = rankingDataStore.GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update() {

        //とりあえずテストでスペースを押したら、ランキングデータの表示を行うようにしてある
        //本来はゲーム終了後何秒間か経過したら？になるのかな
        if (Input.GetKeyDown(KeyCode.Space) && !isRankingSetting) {
            setRankingData();

            setRankingImage();
            isRankingSetting = true;
        }


        if (isRankingSetting) {
            //ランキングに入っていたら、ランキングデータのスクロール処理を行う
            if (isInRanking) {
                moveScrollRankingImage();
            }else {
                //ランキング外
                
            }
        }
    }


    /// <summary>
    /// ランキングデータを設定
    /// </summary>
    private void setRankingData() {

        //ランキングデータの読み込み
        rankingNo = rankingData.Length;
        Debug.Log("ランキングデータの読み込み Ranking No " + rankingNo);
        ReadCsv();

        //今回のスコアがランクインされていたら、順位を変更して書き換える
        if (rankingNo != rankingData.Length) {
            WriteCsv();
            isInRanking = true;
        } else {
        }
    }

    /// <summary>
    /// ランキングデータの読み込み
    /// </summary>
    private void ReadCsv() {
        
        try {
            ReadCSVFile();
        } catch (System.Exception e) {
            // ファイルを開くのに失敗したとき
            System.Console.WriteLine(e.Message);
            Debug.Log("failed");

            WriteCSVFile();
            ReadCSVFile();
        }
    }

    /// <summary>
    /// CSVファイルから読み込み
    /// </summary>
    private void ReadCSVFile() {
        int index = 0;
        StreamReader reader = new StreamReader(RANKING_FILE_PATH);

        // ストリームの末尾まで繰り返す
        while (reader.Peek() > -1) {

            // ファイルから一行読み込む
            var lineValue = int.Parse(reader.ReadLine());

            //読み込んだスコアよりも今回のスコアのほうが大きい場合、
            //あとでそのランキングの場所に挿入するために、ランキングNoを保存しておく
            Debug.Log("rankingNo " + rankingNo);
            Debug.Log("lineValue " + lineValue + " score " + score);
            if (rankingNo == rankingData.Length && lineValue < score) rankingNo = index;

            rankingData[index] = lineValue;
            index++;
        }

        Debug.Log("success");
        reader.Close();
    }

    /// <summary>
    /// ランキングデータの書き込み
    /// </summary>
    private void WriteCsv() {

        //今回ランクインされる順位以降の順位を１つ下げて、該当の順位に今回のスコアを入れる
        for (int index = rankingData.Length - 2; index >= rankingNo; index--) {
            if (rankingData[index + 1] != rankingData[index]) rankingData[index + 1] = rankingData[index];
        }
        rankingData[rankingNo] = score;

        //書き込み処理
        try {
            WriteCSVFile();

        } catch (System.Exception e) {
            // ファイルを開くのに失敗したときエラーメッセージを表示
            System.Console.WriteLine(e.Message);
            Debug.Log("Write failed");
        }
    }

    /// <summary>
    /// CSVファイルへの書き込み
    /// </summary>
    private void WriteCSVFile() {
        StreamWriter writer = new StreamWriter(RANKING_FILE_PATH, false);

        for (int i = 0; i < rankingData.Length; i++) writer.WriteLine(rankingData[i]);

        writer.Flush();
        writer.Close();
        Debug.Log("Write success");
    }



    /// <summary>
    /// ランキング順位の生成
    /// </summary>
    private void setRankingImage() {
        for(int i = 0; i < rankingData.Length; i++) {
            var pos = rankingImagePrefab.transform.position;
            pos.y += (rankingImagePrefab.GetComponent<RectTransform>().sizeDelta.y + rankingImage_AdjustPosition) * i;
            var rankingImageObj = (GameObject)Instantiate(rankingImagePrefab,pos,Quaternion.identity);

            rankingImageObj.transform.SetParent(rankingDataStore.transform,false);
            rankingImageObj.name = rankingImagePrefab.name + "_" + i;
        }

        //inRankingObject = rankingDataStore.transform.GetChild(rankingNo).gameObject;

        inRankingObjectRect = rankingDataStore.transform.GetChild(rankingNo).GetComponent<RectTransform>();
    }

    /// <summary>
    /// ランキングデータのスクロール処理
    /// </summary>
    private void moveScrollRankingImage() {

        if (inRankingObjectRect.position.y > 0.0f) {
            var storePos = storeRect.position;
            storePos.y -= Time.deltaTime * scrollSpeed;
            storeRect.position = storePos;
        }
    }
}
