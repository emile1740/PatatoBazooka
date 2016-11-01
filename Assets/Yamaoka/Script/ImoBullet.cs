using UnityEngine;
using System.Collections;

public class ImoBullet : MonoBehaviour {
    private Collider myCol;
    private Rigidbody rigid;
    [SerializeField]
    private float rotPower = 200;

	void Awake () {
        myCol = GetComponent<Collider>();
        rigid = GetComponent<Rigidbody>();
	}

    void OnEnable()
    {
        myCol.isTrigger = false;
        var vec = Vector3.zero;
        vec.x = Random.Range(-rotPower, rotPower);
        vec.y = Random.Range(-rotPower, rotPower);
        vec.z = Random.Range(-rotPower, rotPower);


        rigid.angularVelocity = vec;
    }
	
	// Update is called once per frame
	void Update () {
        //transform.Rotate()
        if (transform.position.y < -10.0f)
        {
            gameObject.SetActive(false);
        }
	
	}
    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Pumpkin")
        {
            myCol.isTrigger = true;
        }
    }
}
