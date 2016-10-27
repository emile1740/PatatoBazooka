using UnityEngine;
using System.Collections;

public class ImoAndSmokePool : ObjectPool {
    [SerializeField]
    private GameObject ImoPrefab;
    [SerializeField]
    private GameObject SmokePrefab;

    [SerializeField, Header("開始時にプールする芋の数")]
    private int createImoCount;
    [SerializeField, Header("開始時にプールする煙の数")]
    private int createSmokeCount;

    //開始時に一定量作っておく
    void Awake()
    {
        int max = createImoCount + createSmokeCount;

        GameObject[] temp = new GameObject[max];
        for (int i = 0; i < createImoCount; i++)
        {
            temp[i]=ObjectPool.Instance.GetGameObject(ImoPrefab, Vector3.zero, Quaternion.identity);
        }
        for (int i = createImoCount; i < max; i++)
        {
            temp[i] = ObjectPool.Instance.GetGameObject(SmokePrefab, Vector3.zero, Quaternion.identity);
        }
        foreach (GameObject obj in temp)
        {
            obj.SetActive(false);
        }
    }

}
