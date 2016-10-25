using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PanelScalingManager : MonoBehaviour {
    
    private float scalingTimer;
    [Header("結果表示用パネルを拡大させる時間")]
    public float MAX_SCALING_TIME;

    public Dictionary<RectTransform,Component> panelDic;

    /// <summary>
    /// 値の初期化
    /// </summary>
    public void initialize() {
        scalingTimer = 0.0f;
    }

    // Use this for initialization
    void Start () {
        panelDic = new Dictionary<RectTransform, Component>();
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    /// <summary>
    /// 指定したパネルをKey値とするスクリプトをディクショナリ配列に登録
    /// </summary>
    /// <param name="panel"></param>
    /// <param name="script"></param>
    public void addPanelDictionary(RectTransform panel,Component script) {
        panelDic.Add(panel,script);
    }

    /// <summary>
    /// 指定したパネルを、stからedの大きさに拡大縮小する
    /// </summary>
    /// <param name="panel"></param>
    /// <param name="st"></param>
    /// <param name="ed"></param>
    public void setPanelScaling(RectTransform panel, float st, float ed) {
        scalingTimer += Time.deltaTime;
        var scale = Mathf.Lerp(st, ed, scalingTimer / MAX_SCALING_TIME);
        panel.localScale = new Vector3(scale, scale, scale);

        if (scale == ed) {

            panelDic[panel].SendMessage("PanelScalingEnd");
        }
    }
}
