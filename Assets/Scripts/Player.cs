using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 10f;
    [SerializeField] CinemachineCamera cam;
    [SerializeField] CinemachineInputAxisController axisController;

    public static double lat = 51.25;
    public static double lon = 22.56;

    private InputActions input;
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        input = new InputActions();
        input.PlayerInput.Enable();
    }

    private void Update()
    {
        Move();
        Zoom();
    }

    public void Move()
    {
        Vector2 moveInput = input.PlayerInput.Move.ReadValue<Vector2>().normalized;
        Vector3 moveDir = new Vector3(moveInput.x, 0, moveInput.y);
        moveDir = Quaternion.AngleAxis(cam.transform.eulerAngles.y, Vector3.up) * moveDir;
        transform.Translate(moveDir * Time.deltaTime * movementSpeed, Space.World);
    }
    public void Zoom()
    {
        cam.Lens.FieldOfView = Mathf.Clamp(cam.Lens.FieldOfView - input.PlayerInput.Zoom.ReadValue<Vector2>().y * (cam.Lens.FieldOfView / 10f), 0.01f, 90f);
        axisController.Controllers[0].Input.Gain = cam.Lens.FieldOfView / 60f;
        axisController.Controllers[1].Input.Gain = -cam.Lens.FieldOfView / 60f;
    }

    public void ResetZoom()
    {
        cam.Lens.FieldOfView = 60f;
    }
}
