using UnityEngine;

public class ThrowBodyPart : MonoBehaviour
{
    private readonly float _force = 50.0f;
    public GameObject playerObject;
    public GameObject leftArm;
    public GameObject rightArm;

    public GameObject equippedArm;
    public GameObject playerLeftArm;
    public GameObject playerRightArm;
    private PlayerScript player;

    public float equipRange = 1.5f;

    void Start()
    {
        player = playerObject.GetComponent<PlayerScript>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) EquipArm(leftArm, playerLeftArm);
        if (Input.GetKeyDown(KeyCode.R)) EquipArm(rightArm, playerRightArm);
        if (Input.GetKeyDown(KeyCode.C) && equippedArm != null) ThrowBP(equippedArm);
        if (Input.GetKeyDown(KeyCode.F))
        {
            TryReequipArm(leftArm, playerLeftArm);
            TryReequipArm(rightArm, playerRightArm);
        }
    }

    private void EquipArm(GameObject arm, GameObject playerArm)
    {
        if (arm == null)
        {
            Debug.LogWarning("Attempted to equip a null arm.");
            return;
        }

        if (equippedArm != null && equippedArm == arm)
        {
            Debug.Log("Unequipping arm: " + arm.name);
            equippedArm.SetActive(false);
            ResetArm(playerArm);
            equippedArm = null;
        }
        if (equippedArm != null && equippedArm != arm)
        {
            Debug.Log("Cannot equip two arms at once.");
            return;
        }

        if (!playerArm.activeSelf)
        {
            Debug.Log("No arm to equip.");
            return;
        }

        playerArm.SetActive(false);
        equippedArm = arm;
        equippedArm.transform.position = player.transform.position + player.transform.forward * (arm == leftArm ? 0.6f : 0.6f) + player.transform.right * (arm == leftArm ? 0.5f : -0.5f) + Vector3.up * 1.5f;
        equippedArm.transform.parent = player.transform;
        equippedArm.SetActive(true);
    }

    private void ResetArm(GameObject playerArm)
    {
        if (playerArm == playerLeftArm) playerLeftArm.SetActive(true);
        else if (playerArm == playerRightArm) playerRightArm.SetActive(true);
    }

    private void TryReequipArm(GameObject arm, GameObject playerArm)
    {
        if (arm != null && Vector3.Distance(player.transform.position, arm.transform.position) <= equipRange)
        {
            playerArm.SetActive(true);
            arm.SetActive(false);
            Rigidbody rb = arm.GetComponent<Rigidbody>();
            if (rb == null)
            {
                Debug.LogWarning("The equipped body part lacks a Rigidbody and cannot be thrown.");
                return;
            }
            rb.useGravity = false;
            rb.isKinematic = true;
            arm.transform.parent = player.transform;
            Debug.Log("Re-equipped arm: " + arm.name);
        }
    }

    private void ThrowBP(GameObject bp)
    {
        Rigidbody rb = bp.GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogWarning("The equipped body part lacks a Rigidbody and cannot be thrown.");
            return;
        }

        bp.transform.parent = null;
        rb.useGravity = true;
        rb.isKinematic = false;

        rb.AddForce(player.playerCamera.transform.forward * _force, ForceMode.Impulse);
        equippedArm = null;
    }
}
