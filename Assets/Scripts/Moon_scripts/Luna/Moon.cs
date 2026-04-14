using System;
using UnityEngine;
using static System.Math;
public class Moon : MonoBehaviour
{
    [SerializeField] private Transform moonTransform;
    [SerializeField] private float scaleCorrection = 1.3f;

    public static Vector3 currentMoonDir;
    public static Vector3 peakPositionDir;
    public static double RA;
    public static double Dec;
    public static double Alt;
    public static double Az;
    private void Awake()
    {
        //moonTransform.LookAt(Vector3.zero);
        //moonTransform.Rotate(Vector3.right, -90f);
        //moonTransform.Rotate(Vector3.forward, 90f);

        CalculateCurrPos();
        transform.rotation = Quaternion.LookRotation(currentMoonDir);
        Debug.Log($"RA {RA}, DEC {Dec}, ALT {Alt}, AZ {Az}");
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

        double hourAngle = (Astrotime.GetLST() - RA * PI / 180);

        Alt = Asin(Sin(Player.lat * PI / 180) * Sin(Dec * PI / 180) + Cos(Player.lat * PI / 180) * Cos(Dec * PI / 180) * Cos(hourAngle));
        Az = Atan2(-Sin(hourAngle), Cos(hourAngle) * Sin(Player.lat * PI / 180) - Tan(Dec * PI / 180) * Cos(Player.lat * PI / 180));

        double x = Cos(Alt) * Sin(Az);
        double y = Cos(Alt) * Cos(Az);
        double z = Sin(Alt);

        //double lambda = L + 6.289 * Math.Sin(M * Math.PI / 180);
        //double beta = 5.128 * Math.Sin(F * Math.PI / 180);
        //double epsilon = 23.439 - 0.0000004 * T;

        //RA = Math.Atan2(Math.Sin(lambda * Math.PI / 180) * Math.Cos(epsilon * Math.PI / 180) - Math.Tan(beta * Math.PI / 180) * Math.Sin(epsilon * Math.PI / 180), Math.Cos(lambda * Math.PI / 180)) * 180 / Math.PI;
        //Dec = Math.Asin(Math.Sin(beta * Math.PI / 180) * Math.Cos(epsilon * Math.PI / 180) + Math.Cos(beta * Math.PI / 180) * Math.Sin(epsilon * Math.PI / 180) * Math.Sin(lambda * Math.PI / 180)) * 180 / Math.PI;
        //if (RA < 0)
        //{
        //    RA += 360;
        //}

        //double hourAngle = (Astrotime.GetLST() - RA * Math.PI /180);

        //Alt = Math.Asin(Math.Sin(Player.lat * Math.PI / 180) * Math.Sin(Dec * Math.PI / 180) + Math.Cos(Player.lat * Math.PI / 180) * Math.Cos(Dec * Math.PI / 180) * Math.Cos(hourAngle * Math.PI / 180));
        //Az = Math.Atan2(-Math.Sin(hourAngle * Math.PI / 180), Math.Cos(hourAngle * Math.PI / 180) * Math.Sin(Player.lat * Math.PI / 180) - Math.Tan(Dec * Math.PI / 180) * Math.Cos(Player.lat * Math.PI / 180));

        //double x = Math.Cos(Alt) * Math.Sin(Az);
        //double y = Math.Cos(Alt) * Math.Cos(Az);
        //double z = Math.Sin(Alt);

        currentMoonDir = new Vector3((float)z, (float)y, (float)x);
    }
}
