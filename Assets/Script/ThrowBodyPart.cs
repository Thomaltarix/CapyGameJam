using UnityEngine;
using UnityEngine.UI;

public class ThrowBodyPart : MonoBehaviour
{
    private readonly float _force = 50.0f;
    private readonly Color unactiveColor = new Color(130f / 255f, 130f / 255f, 130f / 255f, 200f / 255f);
    private readonly Color activeColor = new Color(1, 1, 1, 200f / 255f);
    private readonly Color selectedColor = new Color(50f / 255f, 142f / 255f, 1, 200f / 255f);
    public GameObject playerObject;
    public GameObject leftArm;
    public GameObject rightArm;

    public GameObject equippedArm;
    public GameObject playerLeftArm;
    public GameObject playerRightArm;
    private PlayerScript player;

    [Header("Mini player view")]
    public GameObject canvaLeftArm;
    public GameObject canvaRightArm;
    public GameObject canvaLeftLeg;
    public GameObject canvaRightLeg;
    private GameObject activeLimb;

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

        if (playerArm == playerLeftArm) {
            selectLimbView(canvaLeftArm);
        }
        else if (playerArm == playerRightArm) {
            selectLimbView(canvaRightArm);
        }

        playerArm.SetActive(false);
        equippedArm = arm;
        equippedArm.transform.position = player.transform.position + player.transform.forward * (arm == leftArm ? 0.6f : 0.6f) + player.transform.right * (arm == leftArm ? 0.5f : -0.5f) + Vector3.up * 1.5f;
        equippedArm.transform.parent = player.transform;
        equippedArm.SetActive(true);
        //set canva blue
    }

    private void ResetArm(GameObject playerArm)
    {
        if (playerArm == playerLeftArm) playerLeftArm.SetActive(true);
        else if (playerArm == playerRightArm) playerRightArm.SetActive(true);

        if (playerArm == playerLeftArm) activateLimbView(canvaLeftArm);
        else if (playerArm == playerRightArm) activateLimbView(canvaRightArm);
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

            if (playerArm == playerLeftArm) activateLimbView(canvaLeftArm);
            else if (playerArm == playerRightArm) activateLimbView(canvaRightArm);

            //canva white
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

        if (bp == leftArm) desactivateLimbView(canvaLeftArm);
        else if (bp == rightArm) desactivateLimbView(canvaRightArm);

        bp.transform.parent = null;
        rb.useGravity = true;
        rb.isKinematic = false;

        rb.AddForce(player.playerCamera.transform.forward * _force, ForceMode.Impulse);
        equippedArm = null;
    }

    private void desactivateLimbView(GameObject limb)
    {
        Image panelImage = limb.GetComponent<Image>();
        if (panelImage != null) {
            panelImage.color = unactiveColor;
        }
    }

    private void activateLimbView(GameObject limb)
    {
        Image panelImage = limb.GetComponent<Image>();
        if (panelImage != null) {
            panelImage.color = activeColor;
        }
    }

    private void selectLimbView(GameObject limb)
    {
        Image panelImage = limb.GetComponent<Image>();
        if (panelImage != null) {
            panelImage.color = selectedColor;
        }
    }
}
