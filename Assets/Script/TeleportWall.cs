using UnityEngine;
using UnityEngine.SceneManagement;

public class TeleportWall : MonoBehaviour
{
    public string TargetScene;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            SceneManager.LoadScene(this.TargetScene);
            Debug.Log("Loading new scene...");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
