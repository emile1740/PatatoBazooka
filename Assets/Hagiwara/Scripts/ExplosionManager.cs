using UnityEngine;
using System.Collections;

public class ExplosionManager : MonoBehaviour {

    [Header("ゲーム全体のステータスマネージャー")]
    public GameStateManager gameStateManager;

    [Header("花火エフェクトのプレハブ配列")]
    public GameObject[] explosionPrefabList;

    [Header("ターゲットカメラ")]
    public Camera target;
    [Header("ターゲットカメラからどこに生成するかの距離")]
    public float targetDistance;
    [Header("生成時のランダム位置の幅")]
    public float randomWidth;

    [Header("ランダムで生成させる最短時間")]
    public float MIN_RANDOM_TIMER;
    [Header("ランダムで生成させる最長時間")]
    public float MAX_RANDOM_TIMER;
    //ランダムで生成させる時間
    private float randomTimer;

    [Header("ランダムで生成させる最小個数")]
    public int MIN_RANDOM_COUNT;
    [Header("ランダムで生成させる最大個数")]
    public int MAX_RANDOM_COUNT;

    // Use this for initialization
    void Start () {
        randomTimer = Random.Range(MIN_RANDOM_TIMER, MAX_RANDOM_TIMER);
    }

    // Update is called once per frame
    void Update() {

        if (gameStateManager.nowState == GameStateManager.Status.Result) {
            randomTimer -= Time.deltaTime;

            if (randomTimer <= 0.0f) {
                randomTimer = Random.Range(MIN_RANDOM_TIMER, MAX_RANDOM_TIMER);

                generateExplosion();
            }
        }
    }

    /// <summary>
    /// 花火エフェクトを生成
    /// </summary>
    private void generateExplosion() {

        var randomCount = Random.Range(MIN_RANDOM_COUNT, MAX_RANDOM_COUNT + 1);

        //↓現状だと、ランダムで作成しているから、エフェクトが重なる可能性がある
        while (randomCount > 0) {
            var pos = target.transform.TransformDirection(Vector3.forward) * targetDistance;

            pos.x += Random.Range(-randomWidth, randomWidth);
            pos.y += Random.Range(-randomWidth, randomWidth);

            var index = Random.Range(0, explosionPrefabList.Length);

            Instantiate(explosionPrefabList[index], pos, Quaternion.identity);
            randomCount--;
        }
    }
}
