using UnityEngine;
using System.Collections;
using System.IO;

public class FileReader {

    private const string RANKING_FILE_PATH = "Assets/Resources/ranking.csv";

    private Result result;

    public FileReader(Result result) {
        this.result = result;
    }

    /// <summary>
    /// ランキングデータの読み込み
    /// </summary>
    public void ReadCsv() {

        try {
            ReadCSVFile();
        } catch (System.Exception e) {
            // ファイルを開くのに失敗したとき
            System.Console.WriteLine(e.Message);
            Debug.Log("failed");

            var fw = new FileWriter(result);
            fw.WriteCSVFile();

            ReadCSVFile();
        }
    }

    /// <summary>
    /// CSVファイルから読み込み
    /// </summary>
    public void ReadCSVFile() {
        int index = 0;
        StreamReader reader = new StreamReader(RANKING_FILE_PATH);

        // ストリームの末尾まで繰り返す
        while (reader.Peek() > -1) {

            // ファイルから一行読み込む
            var lineValue = int.Parse(reader.ReadLine());

            //読み込んだスコアよりも今回のスコアのほうが大きい場合、
            //あとでそのランキングの場所に挿入するために、ランキングNoを保存しておく
            Debug.Log("rankingNo " + result.rankingNo);
            Debug.Log("lineValue " + lineValue + " score " + result.score);
            if (result.rankingNo == result.rankingData.Length && lineValue < result.score)
                result.rankingNo = index;

            result.rankingData[index] = lineValue;
            index++;
        }

        Debug.Log("success");
        reader.Close();
    }

}
