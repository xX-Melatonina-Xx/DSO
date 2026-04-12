using UnityEngine;

public class JupiterRotation : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 0.1f;
    private void FixedUpdate()
    {
        transform.Rotate(Vector3.up, rotationSpeed);
    }
}
