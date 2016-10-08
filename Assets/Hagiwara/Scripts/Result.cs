﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;

/// <summary>
/// リザルト画面を処理
/// </summary>
public class Result : MonoBehaviour {

    private const string RANKING_FILE_PATH = "Assets/Resources/ranking.csv";

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

        inRankingObjectRect = rankingDataStore.transform.GetChild(rankingNo).GetComponent<RectTransform>();
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

        ////１の位は必ず表示
        viewRankingImage(rankImage.GetChild(2).GetComponent<Image>(), rank % 10);
        rank /= 10;

        //ランキング１００位の場合、１０の位、１００の位を表示
        if ((rankingData.Length - 1) - index == (rankingData.Length - 1)) {
            viewRankingImage(rankImage.GetChild(0).GetComponent<Image>(), 1);
            viewRankingImage(rankImage.GetChild(1).GetComponent<Image>(), 0);
        } else {
            //ランキング１０位以上の場合、１０の位を表示
            if ((rankingData.Length - 1) - index >= 10) {
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