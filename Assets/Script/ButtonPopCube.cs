using UnityEngine;

public class ButtonPopCube : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    bool created = false;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (created) return;
        gameObject.GetComponent<Renderer>().enabled = false;
        GameObject ball = GameObject.CreatePrimitive(PrimitiveType.Cube);
        ball.transform.position = gameObject.transform.position + gameObject.transform.forward + Vector3.up * 4;
        ball.AddComponent<Rigidbody>();
        created = true;
    }
}
