using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AddPumpkinScore : MonoBehaviour {

    [Header("共通オブジェクト")]
    public Common common;

    public int scoreTmp;
    public int targetScore;

    private int dis;

    [Header("スコアイメージリスト")]
    public Image[] scoreImageList;

    [Header("スコアを加算するイメージの親オブジェクト")]
    public GameObject addScoreImageParent;

    [Header("加算スコアのイメージを格納する格納オブジェクトリスト")]
    private GameObject[] scoreImagePool;

    [Header("加算スコアのイメージのプレハブリスト")]
    public GameObject[] scoreImagePrefabList;

    void OnDisable() {
        scoreTmp = 0;
        targetScore = 0;

        scoreImageList[4].sprite = common.count[0];
        scoreImageList[3].sprite = common.count[0];
        scoreImageList[2].sprite = common.count[0];
        scoreImageList[1].sprite = common.count[0];
        scoreImageList[0].sprite = common.count[0];
    }

	// Use this for initialization
	void Start () {
        scoreImagePool = new GameObject[addScoreImageParent.transform.childCount];
        for (int i = 0; i < addScoreImageParent.transform.childCount; i++) {
            scoreImagePool[i] = addScoreImageParent.transform.GetChild(i).gameObject;
        }
    }

    // Update is called once per frame
    void Update() {
        //差を求める
        dis = targetScore - scoreTmp;

        if (dis > 0) {
            if (dis <= 10) {
                scoreTmp++;
            } else {
                scoreTmp += dis / 10;
            }
        }

        scoreImageList[4].sprite = common.count[scoreTmp % 10];
        scoreImageList[3].sprite = common.count[(scoreTmp / 10) % 10];
        scoreImageList[2].sprite = common.count[(scoreTmp / 100) % 10];
        scoreImageList[1].sprite = common.count[(scoreTmp / 1000) % 10];
        scoreImageList[0].sprite = common.count[(scoreTmp / 10000) % 10];
    }

    /// <summary>
    /// スコアイメージの生成
    /// </summary>
    /// <param name="scoreNum"></param>
    public void GenerateScoreImage(int scoreNum) {

        //格納オブジェクトから取得
        var scoreImageObject = GetScoreImage_FormPool(scoreNum);

        if(scoreImageObject == null) {
            //scoreNumをインデックスとして、scoreImagePrefabListから生成
            var pos = scoreImagePool[scoreNum].transform.position;
            var rot = Quaternion.identity;
            scoreImageObject = (GameObject)Instantiate(scoreImagePrefabList[scoreNum],pos,rot);
            scoreImageObject.transform.SetParent(scoreImagePool[scoreNum].transform);

        } else {
            //格納オブジェクトから再表示した場合、初期化処理
            var addScoreImage = scoreImageObject.GetComponent<AddScoreImage>();
            addScoreImage.Initialize();
        }

    }

    /// <summary>
    /// 生成可能状態かどうかをチェックし、生成可能状態(false)ならば、そのオブジェクトを取得
    /// </summary>
    private GameObject GetScoreImage_FormPool(int scoreNum) {
        foreach (Transform child in scoreImagePool[scoreNum].transform) {
            if (!child.gameObject.activeInHierarchy) {
                return child.gameObject;
            }
        }
        return null;
    }

    /// <summary>
    /// 画像を変更させる目標のスコアを設定する
    /// </summary>
    public void setTargetScoreImage(int _score) {
        targetScore = _score;
    }
}
