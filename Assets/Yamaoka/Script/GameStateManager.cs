using UnityEngine;
using System.Collections;

public class GameStateManager : MonoBehaviour {
    public enum Status
    {
        Title,
        Game,
        Result,
        Ranking
    }
    [SerializeField]
    private Status nowState;

    [SerializeField]
    private float gameLimit;
    private float timer;
    [SerializeField]
    private int pumpkinCount = 0;

    [SerializeField]
    private GameObject startButton;
    [SerializeField]
    private GameObject enemy;
    [SerializeField]
    private int pumpNum;
    [SerializeField]
    private float radius = 3.0f;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        switch (nowState)
        {
            case Status.Title:

                break;
            case Status.Game:
                GameUpdate();
                break;
            case Status.Result:

                break;
            case Status.Ranking:

                break;

            default:
                break;
        }
	
	}

    void PumpkinGenerator()
    {
        //確認用
        Transform playerTrans = GameObject.FindObjectOfType<Shoot>().transform;
        float unitAngle = 360.0f / pumpNum ;
        float angle = 30.0f;
        for (int i = 0; i < pumpNum; i++)
        {
            var pos = playerTrans.position + Quaternion.Euler(0.0f,angle,0.0f) * Vector3.forward * radius;
            Instantiate(enemy, pos, Quaternion.identity);
            angle += 60;
        }
    }

    void GameUpdate()
    {
        timer -= Time.deltaTime;
        if (timer < 0.0f)
        {
            GoResult();
        }
    }

    public void KillPumpkin()
    {
        pumpkinCount++;
    }

    public void GoGame()
    {
        nowState = Status.Game;
        timer = gameLimit;
        pumpkinCount = 0;
        startButton.SetActive(false);
        PumpkinGenerator();
    }
    public void GoTitle()
    {
        nowState = Status.Title;
        startButton.SetActive(true);
    }
    public void GoResult()
    {
        nowState = Status.Result;
        startButton.SetActive(true);
    }
    public void GoRanking()
    {
        nowState = Status.Ranking;
    }

}
