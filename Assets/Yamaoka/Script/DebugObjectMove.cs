using UnityEngine;
using System.Collections;

public class DebugObjectMove : MonoBehaviour
{
    [SerializeField]
    private GameObject moveObject;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            moveObject.transform.position +=
                moveObject.transform.rotation * Vector3.forward * 10.0f * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.A))
        {
            moveObject.transform.position +=
                moveObject.transform.rotation * Vector3.left * 10.0f * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.S))
        {
            moveObject.transform.position +=
                moveObject.transform.rotation * Vector3.back * 10.0f * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.D))
        {
            moveObject.transform.position +=
                moveObject.transform.rotation * Vector3.right * 10.0f * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            moveObject.transform.Rotate
                (Vector3.right * 30.0f * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            moveObject.transform.Rotate
                (Vector3.down * 30.0f * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            moveObject.transform.Rotate
                (Vector3.left * 30.0f * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            moveObject.transform.Rotate
                (Vector3.up * 30.0f * Time.deltaTime);
        }
    }
}
