using UnityEngine;
using System.Collections;
using System.IO;

public class FileWriter{

    private const string RANKING_FILE_PATH = "Assets/Resources/ranking.csv";

    private Result result;

    public FileWriter(Result result) {
        this.result = result;
    }

    /// <summary>
    /// ランキングデータの書き込み
    /// </summary>
    public void WriteCsv() {

        //今回ランクインされる順位以降の順位を１つ下げて、該当の順位に今回のスコアを入れる
        for (int index = result.rankingData.Length - 2; index >= result.rankingNo; index--) {
            if (result.rankingData[index + 1] != result.rankingData[index])
                result.rankingData[index + 1] = result.rankingData[index];
        }
        result.rankingData[result.rankingNo] = result.score;

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
    public void WriteCSVFile() {
        StreamWriter writer = new StreamWriter(RANKING_FILE_PATH, false);

        for (int i = 0; i < result.rankingData.Length; i++) writer.WriteLine(result.rankingData[i]);

        writer.Flush();
        writer.Close();
        Debug.Log("Write success");
    }
}
