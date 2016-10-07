﻿using UnityEngine;
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
    private GameObject bullet;
    [SerializeField, Range(1.0f, 100.0f)]
    private float speed;
    [Header("Viveを使用する"), SerializeField]
    private bool isVive;
	// Use this for initialization
    void Start()
    {
        if (isVive)
        {
            rightController = SteamVR_Controller.Input((int)rightTrackedObject.index);
            leftController = SteamVR_Controller.Input((int)leftTrackedObject.index);
        }
    }
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameObject temp = (GameObject)Instantiate(bullet, fireTrans.position, Quaternion.identity);
            temp.GetComponent<Rigidbody>().velocity = cameraTrans.rotation * Vector3.forward * speed;
        }

        if (isVive)
            UpdateVR();
        
	}
    void UpdateVR()
    {
        if (rightController.GetPressDown(SteamVR_Controller.ButtonMask.Trigger))
        {
            GameObject temp = (GameObject)Instantiate(bullet, fireTrans.position, Quaternion.identity);
            temp.GetComponent<Rigidbody>().velocity = rightTransform.rotation * Vector3.forward * speed;

        }
        if (leftController.GetPressDown(SteamVR_Controller.ButtonMask.Trigger))
        {
            GameObject temp = (GameObject)Instantiate(bullet, fireTrans.position, Quaternion.identity);
            temp.GetComponent<Rigidbody>().velocity = leftTransform.rotation * Vector3.forward * speed;

        }
    }
}