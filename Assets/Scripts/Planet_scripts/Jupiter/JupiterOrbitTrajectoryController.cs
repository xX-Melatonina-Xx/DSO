using UnityEngine;

public class JupiterOrbitTrajectoryController : MonoBehaviour
{
    private void Start()
    {
        Vector3 dir = Planet.JUPITER.GetPlanetDir();
        transform.rotation = Quaternion.LookRotation(dir);
    }
    private void FixedUpdate()
    {  
        Vector3 dir = Planet.JUPITER.GetPlanetDir();
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 0.0065f);
    }
}
