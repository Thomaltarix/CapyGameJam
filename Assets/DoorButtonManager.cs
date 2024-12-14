using UnityEngine;
using System.Collections.Generic;

public class DoorButtonManager : MonoBehaviour
{
    private List<bool> doorStates = new List<bool>();

    public GameObject door;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        doorStates.Add(false);
        doorStates.Add(false);
    }

    public void updateDoorState(int id, bool state)
    {
        doorStates[id] = state;
        if (doorStates[0] && doorStates[1])
        {
            door.SetActive(false);
        } else {
            door.SetActive(true);
        }
    }

    void openDoor() {

    }
    // Update is called once per frame
    void Update()
    {

    }

}
