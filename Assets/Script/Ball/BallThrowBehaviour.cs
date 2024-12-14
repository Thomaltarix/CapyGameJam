using Unity.VisualScripting;
using UnityEngine;
using System.Collections;

public class BallBehaviour : MonoBehaviour
{
    private readonly float _ballForce = 50.0f;
    private readonly string _ballTag = "Ball";
    public PlayerScript player;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            ThrowBall(CreateBall());
        }
    }

    private GameObject CreateBall()
    {
        GameObject ball = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        ball.transform.position = player.transform.position + player.transform.forward + Vector3.up * 2;
        ball.AddComponent<Rigidbody>();
        ball.tag = _ballTag;
        return ball;
    }

    private void ThrowBall(GameObject ball)
    {
        ball.GetComponent<Rigidbody>().AddForce(player.playerCamera.transform.forward * _ballForce, ForceMode.Impulse);
    }
}
