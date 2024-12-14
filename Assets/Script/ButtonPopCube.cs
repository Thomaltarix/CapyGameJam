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
        //gameObject.GetComponent<Renderer>().enabled = false;
        GameObject new_cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        new_cube.transform.position = gameObject.transform.position + gameObject.transform.forward + Vector3.up * 4;
        Rigidbody rb = new_cube.AddComponent<Rigidbody>();
        rb.mass = 1.0f;
        created = true;
    }
}


/*    private void OnTriggerEnter(Collider other)
    {
        if (created) return;
        created = true;
        gameObject.GetComponent<Renderer>().enabled = false;
        GameObject new_cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        new_cube.transform.position = gameObject.transform.position + gameObject.transform.forward + Vector3.up * 4;
        new_cube.AddComponent<Rigidbody>();
        Rigidbody rb = new_cube.AddComponent<Rigidbody>();
    }*/