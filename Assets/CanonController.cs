using UnityEngine;

public class CanonController : MonoBehaviour
{
    [SerializeField] private Transform barrel;
    [SerializeField] private Transform launchPoint;
    [SerializeField] private BulletPooling bulletPooling;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private Animator anim;
    [SerializeField] private int lineSegmentCount = 100;
    [SerializeField] private float launchVelocity = 10f;
    [SerializeField] private float timeStep = 0.1f;
    [SerializeField] private GameObject projectilePrefab;

    private bool isAiming = false;
    private void Start()
    {
        lineRenderer.positionCount = 0;
    }

    void Update()
    {
        if(Input.GetMouseButton(0))
        {
            isAiming = true;
            RotateBarrel();
            DrawTrajectory();
        }
        else if (isAiming && Input.GetMouseButtonUp(0))
        {
            isAiming = false;
            Fire();
            lineRenderer.positionCount = 0; // Xóa đường quỹ đạo sau khi bắn
        }
    }

    private void RotateBarrel()
    {
        Vector3 mousePos = Input.mousePosition;
        float distanceToCamera = Camera.main.WorldToScreenPoint(barrel.position).z;
        mousePos.z = distanceToCamera;
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mousePos);
        Vector3 direction = mouseWorldPosition - barrel.position;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        Vector3 eulerAngles = -targetRotation.eulerAngles;
        Debug.Log("eulerAngles: " + eulerAngles);
        if (-eulerAngles.x < 90)
        {
            eulerAngles = Vector3.zero;
        }
        barrel.localEulerAngles = new Vector3(eulerAngles.x, 0, 0);

        // Điều chỉnh launchVelocity dựa trên khoảng cách đến chuột
        launchVelocity = Vector3.Distance(launchPoint.position, mouseWorldPosition) * 1.5f;
    }

    private void DrawTrajectory()
    {
        lineRenderer.positionCount = lineSegmentCount;

        Vector3[] positions = new Vector3[lineSegmentCount];
        Vector3 startingPosition = launchPoint.position;
        
        Vector3 startingVelocity = barrel.forward * launchVelocity;

        for (int i = 0; i < lineSegmentCount; i++)
        {
            float time = i * timeStep;
            positions[i] = startingPosition + -startingVelocity * time + 0.5f * Physics.gravity * time * time;
        }

        lineRenderer.SetPositions(positions);
    }

    private void Fire()
    {
        anim.SetTrigger("Shoot");
        // Tạo viên đạn tại vị trí launchPoint với hướng của barrel
        // GameObject projectile = Instantiate(projectilePrefab, launchPoint.position, barrel.rotation);
        GameObject projectile = bulletPooling.GetBullet();
        projectile.transform.position = launchPoint.position;
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        rb.velocity = barrel.forward * - launchVelocity;
    }
}
