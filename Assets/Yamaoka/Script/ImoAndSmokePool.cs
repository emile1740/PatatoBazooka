using UnityEngine;
using System.Collections;

public class ImoAndSmokePool : ObjectPool {
    [SerializeField]
    private GameObject ImoPrefab;
    [SerializeField]
    private GameObject HitEffectPrefab;
    [SerializeField]
    private GameObject[] SmokePrefab;

    [SerializeField, Header("開始時にプールする芋の数")]
    private int createImoCount;
    [SerializeField, Header("開始時にプールするヒットエフェクトの数")]
    private int createHitEffectCount;
    [SerializeField, Header("開始時にプールする煙の数")]
    private int createSmokeCount;

    //開始時に一定量作っておく
    void Awake()
    {
        int sec = createImoCount + createHitEffectCount;
        int max = sec + createSmokeCount * SmokePrefab.Length;

        GameObject[] temp = new GameObject[max];
        for (int i = 0; i < createImoCount; i++)
        {
            temp[i] = ObjectPool.Instance.GetGameObject(ImoPrefab, Vector3.zero, Quaternion.identity);
        }
        for (int i = createImoCount; i < sec; i++)
        {
            temp[i] = ObjectPool.Instance.GetGameObject(HitEffectPrefab, Vector3.zero, Quaternion.identity);
        }
        for (int i = sec; i < max; i++)
        {
            temp[i] = ObjectPool.Instance.GetGameObject(SmokePrefab[(i - sec) % 4], Vector3.zero, Quaternion.identity);
        }
        foreach (GameObject obj in temp)
        {
            obj.SetActive(false);
        }
    }

}
