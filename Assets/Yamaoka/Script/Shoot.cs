using UnityEngine;
using System.Collections;

public class Shoot : MonoBehaviour {
    //Viveのコントローラーの状態取得用
    [SerializeField]
    private SteamVR_TrackedObject rightTrackedObject;
    [SerializeField]
    private SteamVR_TrackedObject leftTrackedObject;
    private SteamVR_Controller.Device rightController;
    private SteamVR_Controller.Device leftController;
    [SerializeField]
    private Transform rightTransform;
    [SerializeField]
    private Transform leftTransform;

    [SerializeField]
    private Transform cameraTrans;
    [SerializeField]
    private Transform fireTrans;
    [SerializeField]
    private AudioSource fireAudio;

    [SerializeField]
    private GameObject bullet;
    [SerializeField, Range(1.0f, 100.0f)]
    private float speed;
    [Header("Viveを使用する"), SerializeField]
    private bool isVive;

    [SerializeField, Header("振動する時間"), Range(0.1f, 1.0f)]
    private float vibTime;
    private float timerRightVib = 0.0f;
    private float timerLeftVib = 0.0f;
    private bool isRightVib = false;
    private bool isLeftVib = false;
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameObject temp = ObjectPool.Instance.GetGameObject(bullet, fireTrans.position, Quaternion.Euler(fireTrans.eulerAngles + Vector3.left * 90.0f));
            temp.GetComponent<Rigidbody>().velocity = cameraTrans.rotation * Vector3.forward * speed;
            fireAudio.Play();
        }

        if (isVive)
            UpdateVR();
        
	}
    void UpdateVR()
    {
        rightController = SteamVR_Controller.Input((int)rightTrackedObject.index);
        leftController = SteamVR_Controller.Input((int)leftTrackedObject.index);
        if (rightController.GetPressDown(SteamVR_Controller.ButtonMask.Trigger))
        {
            //コントローラーの振動(boolで管理)
            isRightVib = true;
            //芋発生
            GameObject temp = ObjectPool.Instance.GetGameObject(bullet, fireTrans.position, Quaternion.Euler(fireTrans.eulerAngles - fireTrans.localEulerAngles + Vector3.left * 90.0f));
            //芋発射
            temp.GetComponent<Rigidbody>().velocity = fireTrans.rotation * Vector3.forward * speed;
            //発射音
            fireAudio.Play();
        }
        else if (leftController.GetPressDown(SteamVR_Controller.ButtonMask.Trigger))
        {
            isLeftVib = true;
            GameObject temp = ObjectPool.Instance.GetGameObject(bullet, fireTrans.position, Quaternion.Euler(fireTrans.eulerAngles - fireTrans.localEulerAngles + Vector3.left * 90.0f));
            temp.GetComponent<Rigidbody>().velocity = fireTrans.rotation * Vector3.forward * speed;
            fireAudio.Play();
        }

        if (isRightVib)
        {
            timerRightVib += Time.deltaTime;
            if (vibTime < timerRightVib)
            {
                isRightVib = false;
                timerRightVib = 0.0f;
            }

            rightController.TriggerHapticPulse(2000);
        }
        if (isLeftVib)
        {
            timerLeftVib += Time.deltaTime;
            if (vibTime < timerLeftVib)
            {
                isLeftVib = false;
                timerLeftVib = 0.0f;
            }

            leftController.TriggerHapticPulse(2000);
        }

    }
}
