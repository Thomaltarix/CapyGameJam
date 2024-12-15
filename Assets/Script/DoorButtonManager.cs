using UnityEngine;
using System.Collections.Generic;

public class DoorButtonManager : MonoBehaviour
{
    private List<bool> doorStates = new List<bool>();

    public GameObject door;

    private Vector3 originalPos;

    private bool shown = true;

    public float transormValue;

    void Start()
    {
        doorStates.Add(false);
        doorStates.Add(false);
        originalPos = door.transform.position;
    }

    public void updateDoorState(int id, bool state)
    {
        doorStates[id] = state;
        if (doorStates[0] && doorStates[1])
        {
            // door.SetActive(false);
            shown = false;
        } else {
            // door.SetActive(true);
            shown = true;
        }
    }

    void openDoor() {

    }

    void Update()
    {
        if (door.transform.position.y >= originalPos.y - transormValue && !shown)
            door.transform.position = new Vector3(door.transform.position.x, door.transform.position.y - 0.1f, door.transform.position.z);
        if (door.transform.position.y < originalPos.y && shown)
            door.transform.position = new Vector3(door.transform.position.x, door.transform.position.y + 0.1f, door.transform.position.z);
    }
}
