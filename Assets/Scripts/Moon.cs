using System;
using UnityEngine;
using static System.Math;
public class Moon : MonoBehaviour
{
    [SerializeField] private Transform rotationPivotTransform;
    [SerializeField] private Transform prefabTransform;
    [SerializeField] private float scaleCorrection = 1.3f;
    [SerializeField] private float prefabDistance = 800;
    [SerializeField] private bool displayDebugInfo = false;

    public static Vector3 currentMoonPos;
    public static Vector3 orbitCenter;
    public static Vector3 orbitCenterNormal;
    public static double RA;
    public static double Dec;
    public static double Alt;
    public static double Az;
    public static double hourAngle;

    private static float distanceFactor;
    private float tanQuarterDegree = 0.0043633231f;

    private void Awake()
    {
        UpdateAndSetOrbitPath();
        transform.SetPositionAndRotation(orbitCenter, Quaternion.LookRotation(currentMoonPos * distanceFactor - transform.position, orbitCenterNormal));

    }
    void FixedUpdate()
    {
        UpdateAndSetOrbitPath();
        rotationPivotTransform.localPosition = prefabDistance * Vector3.forward;
        prefabTransform.localScale = 100 * prefabDistance * scaleCorrection * tanQuarterDegree * Vector3.one;
    }

    private void UpdateAndSetOrbitPath()
    {
        double T = Astrotime.GetT();

        double L = (218.316 + 481267.8813 * T);
        L = (L % 360 + 360) % 360;
        double M = (134.963 + 477198.8676 * T);
        M = (M % 360 + 360) % 360;
        double F = (93.272 + 483202.0175 * T);
        F = (F % 360 + 360) % 360;

        double lambda = L + 6.289 * Sin(M * PI / 180);
        double beta = 5.128 * Sin(F * PI / 180);
        double epsilon = 23.439 - 0.0000004 * T;

        RA = (Atan2(Sin(lambda * PI / 180) * Cos(epsilon * PI / 180) - Tan(beta * PI / 180) * Sin(epsilon * PI / 180), Cos(lambda * PI / 180))) * 180 / PI;
        Dec = (Asin(Sin(beta * PI / 180) * Cos(epsilon * PI / 180) + Cos(beta * PI / 180) * Sin(epsilon * PI / 180) * Sin(lambda * PI / 180))) * 180 / PI;

        RA = (RA + 360) % 360;

        hourAngle = (Astrotime.GetLST() - RA * PI / 180);
        hourAngle = (hourAngle + PI) % (2 * PI);
        if (hourAngle < 0) hourAngle += 2 * PI;
        hourAngle -= PI;

        currentMoonPos = GetPosByHourAngle(hourAngle);
        Vector3 southCulmination = GetPosByHourAngle(0);
        Vector3 northCulmination = GetPosByHourAngle(PI);
        Vector3 westCulmination = GetPosByHourAngle(PI / 2);
        Vector3 eastCulmination = GetPosByHourAngle(3 * PI / 2);

        distanceFactor = prefabDistance / southCulmination.magnitude;
        orbitCenter = (southCulmination + northCulmination + westCulmination + eastCulmination) / 4 * distanceFactor;
        orbitCenterNormal = orbitCenter.normalized;
        if(orbitCenter.y < 0)
        {
            orbitCenterNormal *= -1;
        }

        transform.SetPositionAndRotation(orbitCenter, Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(currentMoonPos * distanceFactor - transform.position, orbitCenterNormal), 0.0065f));

        if (displayDebugInfo)
        {
            Debug.DrawLine(transform.position, rotationPivotTransform.position, Color.white);
            Debug.DrawLine(Vector3.zero, southCulmination * distanceFactor, Color.red);
            Debug.DrawLine(Vector3.zero, northCulmination * distanceFactor, Color.green);
            Debug.DrawLine(Vector3.zero, westCulmination * distanceFactor, Color.blue);
            Debug.DrawLine(Vector3.zero, eastCulmination * distanceFactor, Color.yellow);
            Debug.DrawLine(Vector3.zero, orbitCenter, Color.purple);
            Debug.DrawLine(southCulmination * distanceFactor, northCulmination * distanceFactor, Color.cyan);
            Debug.DrawLine(westCulmination * distanceFactor, eastCulmination * distanceFactor, Color.orange);
            Debug.Log($"RA {RA}, DEC {Dec}, ALT {Alt * 180 / PI}, AZ {Az * 180 / PI}, HA {(hourAngle * 180 / PI) / 15}");
        }
    }
    public static Vector3 GetPosByHourAngle(double ha)
    {
        Alt = Asin(Sin(Player.lat * PI / 180) * Sin(Dec * PI / 180) + Cos(Player.lat * PI / 180) * Cos(Dec * PI / 180) * Cos(ha));
        Az = Atan2(Sin(ha), Cos(ha) * Sin(Player.lat * PI / 180) - Tan(Dec * PI / 180) * Cos(Player.lat * PI / 180));
        Az += PI;

        if (Az < 0) Az += 2 * PI;

        if (Az > 2 * PI) Az -= 2 * PI;

        double x = Cos(Alt) * Sin(Az);
        double z = Cos(Alt) * Cos(Az);
        double y = Sin(Alt);

        return new Vector3((float)-z, (float)y, (float)x);
    }
}
