using UnityEngine;

public class ButtonScript : MonoBehaviour
{
    public Material baseColor;
    public Material triggerColor;
    private bool isTrigger = false;

    void Start()
    {
    }

    private void changeColor() {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null && isTrigger)
        {
            renderer.material = triggerColor;
        }
        else if (renderer != null && !isTrigger)
        {
            renderer.material = baseColor;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        isTrigger = true;
        changeColor();
    }

    private void OnTriggerExit(Collider other)
    {
        isTrigger = false;
        changeColor();
    }
}
