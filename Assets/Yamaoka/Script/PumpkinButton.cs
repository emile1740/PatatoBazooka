using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class PumpkinButton : MonoBehaviour {
    [SerializeField]
    private GameStateManager.Status destination;
    [HideInInspector]
    public UnityEvent CollisionPotato;
    private bool onceCollision = false;
    void Start()
    {
        var temp = GameObject.FindObjectOfType<GameStateManager>();
        switch (destination)
        {
            case GameStateManager.Status.Title:
                CollisionPotato.AddListener(temp.GoTitle);
                break;
            case GameStateManager.Status.Game:
                CollisionPotato.AddListener(temp.GoGame);
                break;
            case GameStateManager.Status.Result:
                CollisionPotato.AddListener(temp.GoResult);
                break;
            case GameStateManager.Status.Ranking:
                CollisionPotato.AddListener(temp.GoRanking);
                break;

            default:
                break;
        }
    }

    void OnEnable()
    {
        onceCollision = false;
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Potato"&&!onceCollision)
        {
            onceCollision = true;
            CollisionPotato.Invoke();
        }

    }

}
