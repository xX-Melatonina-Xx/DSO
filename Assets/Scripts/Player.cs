using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
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
        Zoom();
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
