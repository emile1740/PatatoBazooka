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
            GameObject temp = ObjectPool.Instance.GetGameObject(bullet, fireTrans.position, Quaternion.Euler(fireTrans.eulerAngles + Vector3.left * 90.0f));
            temp.GetComponent<Rigidbody>().velocity = fireTrans.rotation * Vector3.forward * speed;
            fireAudio.Play();
        }
        if (leftController.GetPressDown(SteamVR_Controller.ButtonMask.Trigger))
        {
            GameObject temp = ObjectPool.Instance.GetGameObject(bullet, fireTrans.position, Quaternion.Euler(fireTrans.eulerAngles + Vector3.left * 90.0f));
            temp.GetComponent<Rigidbody>().velocity = fireTrans.rotation * Vector3.forward * speed;
            fireAudio.Play();
        }
    }
}
