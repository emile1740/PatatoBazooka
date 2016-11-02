using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AddScoreImage : MonoBehaviour {

    private float fadeTimer;
    [Header("何秒間かけてフェードアウトするか")]
    public float MAX_FADE_TIME;

    [Header("上方向に移動する速度")]
    public float upSpeed;
    private Vector3 defaultPosition;

    private Text currentImage;

    private bool state;
    private Color targetColor;

    //再表示されたときに初期値にもどす
    public void Initialize() {
        //初期位置に戻す
        transform.localPosition = defaultPosition;

        fadeTimer = 0.0f;

        //アルファ値を戻す
        var color = currentImage.color;
        color.a = 1.0f;
        currentImage.color = color;

        state = true;
    }

    // Use this for initialization
    void Start () {
        defaultPosition = transform.localPosition;
        currentImage = GetComponent<Text>();
        state = true;

        targetColor = currentImage.color;
        targetColor.a = 0.0f;
    }
	
	// Update is called once per frame
	void Update () {

        //移動可能状態ならば、フェードアウトしながら上方向に移動させる
        if (state) {
            var pos = transform.localPosition;
            pos.y += Time.deltaTime * upSpeed;
            transform.localPosition = pos;

            fadeTimer += Time.deltaTime;
            currentImage.color = Color.Lerp(currentImage.color, targetColor, fadeTimer / MAX_FADE_TIME);

            if (currentImage.color == targetColor) {
                state = false;
            }
        }
    }
}
