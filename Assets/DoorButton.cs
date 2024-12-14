using UnityEngine;

public class DoorButton : MonoBehaviour
{
    public int id;
    public DoorButtonManager manager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        manager.updateDoorState(id, true);
    }
    void OnTriggerExit(Collider other)
    {
        manager.updateDoorState(id, false);
    }
}
