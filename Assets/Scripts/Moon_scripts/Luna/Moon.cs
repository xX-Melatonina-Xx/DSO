using System;
using UnityEngine;
using static System.Math;
public class Moon : MonoBehaviour
{
    [SerializeField] private Transform moonTransform;
    [SerializeField] private float scaleCorrection = 1.3f;
    [SerializeField] private bool displayDebugInfo = false;

    public static Vector3 currentMoonDir;
    public static Vector3 peakPositionDir;
    public static double RA;
    public static double Dec;
    public static double Alt;
    public static double Az;
    public static double hourAngle;

    private void Awake()
    {
        CalculateCurrPos();
        transform.rotation = Quaternion.LookRotation(currentMoonDir);
        


        
        //hourAngle = 0;
        //double peakAlt = Math.Asin(Math.Sin(Player.lat * Math.PI / 180) * Math.Sin(Dec * Math.PI / 180) + Math.Cos(Player.lat * Math.PI / 180) * Math.Cos(Dec * Math.PI / 180) * Math.Cos(hourAngle * Math.PI / 180));
        //double peakAz = Math.Atan2(-Math.Sin(hourAngle * Math.PI / 180), Math.Cos(hourAngle * Math.PI / 180) * Math.Sin(Player.lat * Math.PI / 180) - Math.Tan(Dec * Math.PI / 180) * Math.Cos(Player.lat * Math.PI / 180));

        //double peakX = Math.Cos(peakAlt) * Math.Sin(peakAz);
        //double peakY = Math.Cos(peakAlt) * Math.Cos(peakAz);
        //double peakZ = Math.Sin(peakAlt);
        //peakPositionDir = new Vector3((float)peakY, (float)peakZ, (float)peakX);


        //
        //float elevationUnits = Vector3.Distance(currentMoonPosition, new Vector3(currentMoonPosition.x, 0, currentMoonPosition.z));
        //transform.Translate(new Vector3(0, elevationUnits, 0));
        //Debug.Log($"Moon elevation units: {elevationUnits}");
        //Debug.Log($"Moon position: {currentMoonPosition}");
        //

        
    }
    void FixedUpdate()
    {
        CalculateCurrPos();
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(currentMoonDir), 0.0065f);

        float tanQuarterDegree = 0.004363f;
        moonTransform.localScale = Vector3.one * Vector3.Distance(moonTransform.position, Vector3.zero) * tanQuarterDegree * 100 * scaleCorrection;
    }

    private void CalculateCurrPos()
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

        Alt = Asin(Sin(Player.lat * PI / 180) * Sin(Dec * PI / 180) + Cos(Player.lat * PI / 180) * Cos(Dec * PI / 180) * Cos(hourAngle));
        Az = Atan2(Sin(hourAngle), Cos(hourAngle) * Sin(Player.lat * PI / 180) - Tan(Dec * PI / 180) * Cos(Player.lat * PI / 180));
        Az += PI;

        if (Az < 0) Az += 2 * PI;

        if (Az > 2 * PI) Az -= 2 * PI;

        double x = Cos(Alt) * Sin(Az);
        double z = Cos(Alt) * Cos(Az);
        double y = Sin(Alt);
           
        if (displayDebugInfo)
            Debug.Log($"RA {RA}, DEC {Dec}, ALT {Alt * 180 / PI}, AZ {Az * 180 / PI}, HA {(hourAngle * 180 / PI) / 15}");

        currentMoonDir = new Vector3((float)-z, (float)y, (float)x).normalized;

    }
}
