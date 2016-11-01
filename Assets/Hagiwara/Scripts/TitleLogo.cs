using UnityEngine;
using System.Collections;
using System;

public class TitleLogo : MonoBehaviour {

    private float scale;

    [Header("サイズ変更の速度")]
    public float scalingSpeed;

    [Header("最小サイズ")]
    public float MIN_SCALE;

    [Header("最大サイズ")]
    public float MAX_SCALE;

    private SCALING scaling;
    private enum SCALING {
        EXPAND = 1,
        REDUCTION = -1
    }

    // Use this for initialization
    void Start () {
        scale = 1.0f;
        scaling = SCALING.REDUCTION;
    }

    // Update is called once per frame
    void Update () {

        if ((scaling == SCALING.EXPAND && scale > MAX_SCALE) ||
            (scaling == SCALING.REDUCTION && scale < MIN_SCALE)) {

            scaling = (SCALING)Enum.ToObject(typeof(SCALING), (int)scaling * -1);
        }

        scale += Time.deltaTime * scalingSpeed * (int)scaling;
        transform.localScale = new Vector3(scale, scale, scale);

    }
}
