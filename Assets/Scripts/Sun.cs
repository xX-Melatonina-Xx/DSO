using System;
using System.Security.Cryptography;
using UnityEngine;

public class Sun : MonoBehaviour
{
    [SerializeField] private Transform directionalLightTransform;
    void Start()
    {
        directionalLightTransform.forward = GetSunDir();
    }

    void Update()
    {
        directionalLightTransform.forward = GetSunDir();
    }

    private static Vector3 GetSunDir()
    {
        float d = (float)Astrotime.GetDaysFromJ2000();

        float L = 280.460f + 0.9856474f * d;
        L = (L + 360) % 360;

        float g = 357.528f + 0.9856003f * d;
        g = (g + 360) % 360;

        float lambda = L + 1.915f * Mathf.Sin(g * Mathf.Deg2Rad) + 0.020f * Mathf.Sin(2 * g * Mathf.Deg2Rad);

        float epsilon = 23.439f;

        float RA = Mathf.Atan2(Mathf.Cos(epsilon * Mathf.Deg2Rad) * Mathf.Sin(lambda * Mathf.Deg2Rad), Mathf.Cos(lambda * Mathf.Deg2Rad));
        float DEC = Mathf.Asin(Mathf.Sin(epsilon * Mathf.Deg2Rad) * Mathf.Sin(lambda * Mathf.Deg2Rad));

        RA = (RA + Mathf.PI) % (2 * Mathf.PI);

        float HA = (float)Astrotime.GetLST() - RA;

        float Alt = Mathf.Asin(Mathf.Sin(DEC) * Mathf.Sin((float)Player.lat * Mathf.Deg2Rad) + Mathf.Cos(DEC) * Mathf.Cos((float)Player.lat * Mathf.Deg2Rad) * Mathf.Cos(HA));
        float Az = Mathf.Atan2(-Mathf.Cos(DEC) * Mathf.Cos((float)Player.lat * Mathf.Deg2Rad) * Mathf.Sin(HA), Mathf.Sin(DEC) - Mathf.Sin((float)Player.lat * Mathf.Deg2Rad) * Mathf.Sin(Alt));

        if (Az < 0) Az += 2 * Mathf.PI;

        if (Az > 2 * Mathf.PI) Az -= 2 * Mathf.PI;

        double x = Mathf.Cos(Alt) * Mathf.Sin(Az);
        double z = Mathf.Cos(Alt) * Mathf.Cos(Az);
        double y = Mathf.Sin(Alt);

        return new Vector3((float)-z, (float)y, (float)x);
    }
}
