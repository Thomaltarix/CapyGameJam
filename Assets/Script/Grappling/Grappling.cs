using UnityEngine;

class Grappling : MonoBehaviour
{
    [Header("References")]
    private PlayerScript player;
    public Transform cam;
    public Transform hook;
    public LayerMask isGrappeable;
    public LineRenderer lineRenderer;

    [Header("Grappling")]
    public float maxDistance;
    public float delayTime;
    public float overshootYAxis;

    private Vector3 grapplePoint;

    [Header("Grappling State")]
    public float cooldown;
    private float cooldownTimer;

    [Header("Input")]
    public KeyCode grapplingKey = KeyCode.Mouse1;
    private bool _isGrappling = false;

    private void Start()
    {
        player = GetComponent<PlayerScript>();
    }

    private void Update()
    {
        if (Input.GetKey(grapplingKey))
        {
            StartGrappling();
        }

        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
        }
    }

    private void LateUpdate()
    {
        if (_isGrappling)
        {
            lineRenderer.SetPosition(0, hook.position);
        }
    }

    private void StartGrappling()
    {
        if (cooldownTimer > 0)
        {
            return;
        }
        _isGrappling = true;
        player.freeze = true;
        if (Physics.Raycast(cam.position, cam.forward, out RaycastHit hit, maxDistance, isGrappeable))
        {
            grapplePoint = hit.point;

            Invoke(nameof(ExecGrapple), delayTime);
        } else {
            grapplePoint = cam.position + cam.forward * maxDistance;
            Invoke(nameof(StopGrapple), delayTime);
        }
        lineRenderer.enabled = true;
        lineRenderer.SetPosition(1, grapplePoint);
    }

    private void ExecGrapple()
    {
        player.freeze = false;

        Vector3 lowestPoint = new Vector3(transform.position.x, transform.position.y - 1f, transform.position.z);
        float grapplePointRelativeYPos = grapplePoint.y - lowestPoint.y;
        float highestPointOnArc = grapplePointRelativeYPos + overshootYAxis;

        if (grapplePointRelativeYPos < 0)
        {
            highestPointOnArc = overshootYAxis;
        }
        player.JumpToPosition(grapplePoint, highestPointOnArc);

        Invoke(nameof(StopGrapple), 1f);
    }

    public void StopGrapple()
    {
        _isGrappling = false;
        player.freeze = false;

        cooldownTimer = cooldown;
        lineRenderer.enabled = false;
    }

}