using UnityEngine;
using System.Collections;

public class ImoBullet : MonoBehaviour {
    private Collider myCol;

	void Awake () {
        myCol = GetComponent<Collider>();
	}

    void OnEnable()
    {
        myCol.isTrigger = false;
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
