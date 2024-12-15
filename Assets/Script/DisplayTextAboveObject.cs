using UnityEngine;
using TMPro;

public class DisplayTextAboveObject : MonoBehaviour
{
    public Transform camTransform;
    public Transform unit;
    public Transform worldSpaceCanvas;

    public Vector3 offset = new Vector3(0, 2, 0);

    void Start()
    {
        camTransform = Camera.main.transform;
        unit = transform.parent;
        worldSpaceCanvas = GameObject.Find("WorldSpaceCanva").transform;
        transform.SetParent(worldSpaceCanvas);
    }

    void Update()
    {
        transform.rotation = Quaternion.LookRotation(transform.position - camTransform.position);
        transform.position = unit.position + offset;
    }
}
