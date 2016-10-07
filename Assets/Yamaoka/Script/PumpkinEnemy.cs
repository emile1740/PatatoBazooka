using UnityEngine;
using System.Collections;
using UnityEngine.Events;


public class PumpkinEnemy : MonoBehaviour {
    [HideInInspector]
    public UnityEvent CollisionPotato;
    
	// Use this for initialization
	void Start () {
        var temp = GameObject.FindObjectOfType<GameStateManager>();
        CollisionPotato.AddListener(temp.KillPumpkin);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Potato")
        {
            CollisionPotato.Invoke();
            Destroy(gameObject);
        }

    }
}
