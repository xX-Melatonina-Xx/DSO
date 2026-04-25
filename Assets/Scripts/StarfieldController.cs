using JetBrains.Annotations;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using TMPro;
using UnityEngine;

public class StarfieldController : MonoBehaviour
{
    [SerializeField] private bool calculateStars = true;
    [SerializeField] private float difference = 0.4f; // im wiekszy float, tym wieksza roznica
    [SerializeField] private float scaleModifier = 0.003f;
    public float distance;
    public float magnitudeLimit;

    public Mesh starMesh;
    public Material starMaterial;

    List<Matrix4x4[]> batches = new List<Matrix4x4[]>();

    void Start()
    {
        double[] altAzPolaris = RaDecDegToAltAzRad(37.946143, 89.264138);
        double AltRadPolaris = altAzPolaris[0];
        double AzRadPolaris = altAzPolaris[1];

        double xPolaris = distance * Math.Cos(AltRadPolaris) * Math.Cos(AzRadPolaris);
        double yPolaris = distance * Math.Cos(AltRadPolaris) * Math.Sin(AzRadPolaris);
        double zPolaris = distance * Math.Sin(AltRadPolaris);

        gameObject.transform.rotation = Quaternion.LookRotation(new Vector3((float)-xPolaris, (float)zPolaris, (float)yPolaris));

        if (calculateStars)
        {
            List<Matrix4x4> allMatrices = new List<Matrix4x4>();
            var entries = File.ReadAllLines("Assets\\Resources\\Data\\starfield_deep.csv");

            foreach (var entry in entries)
            {
                var fields = entry.Split(',');

                if (double.Parse(fields[3], CultureInfo.InvariantCulture) <= magnitudeLimit)
                {
                    double RAdeg = (double.Parse(fields[1], CultureInfo.InvariantCulture));
                    double DECdeg = (double.Parse(fields[2], CultureInfo.InvariantCulture));

                    double[] altAz = RaDecDegToAltAzRad(RAdeg, DECdeg);
                    double AltRad = altAz[0];
                    double AzRad = altAz[1];

                    double x = distance * Math.Cos(AltRad) * Math.Cos(AzRad);
                    double y = distance * Math.Cos(AltRad) * Math.Sin(AzRad);
                    double z = distance * Math.Sin(AltRad);
                    float mag = float.Parse(fields[3], CultureInfo.InvariantCulture);

                    float baseSize = distance * scaleModifier;
                    float scale = baseSize * Mathf.Pow(2f, -mag * difference);

                    allMatrices.Add(Matrix4x4.TRS(new Vector3((float)-x, (float)z, (float)y), Quaternion.identity, Vector3.one * scale));
                }
            }

            Debug.Log($"Total star instances: {allMatrices.Count}");
            int batchSize = 1023;
            for (int i = 0; i < allMatrices.Count; i += batchSize)
            {
                int count = Mathf.Min(batchSize, allMatrices.Count - i);
                batches.Add(allMatrices.GetRange(i, count).ToArray());
            }
        }
    }

    private void Update()
    {
        for (int i = 0; i < batches.Count; i++)
        {
            Graphics.DrawMeshInstanced(starMesh, 0, starMaterial, batches[i]);
        }
    }

    public static double[] RaDecDegToAltAzRad(double RAdeg, double DECdeg)
    {
        double lonHours = Player.lon / 15.0;
        double LST = Astrotime.GetCurrGMST() + lonHours;
        LST = LST % 24.0;
        double LSTdeg = LST * 15.0;

        double HA = (LSTdeg - RAdeg) % 360;

        if (HA < 0)
        {
            HA += 360.0;
        }

        double DECrad = DECdeg * (Math.PI / 180.0);
        double latRAD = Player.lat * (Math.PI / 180.0);
        double HArad = HA * (Math.PI / 180.0);

        double AltRad = Math.Asin((Math.Sin(DECrad) * Math.Sin(latRAD)) + (Math.Cos(DECrad) * Math.Cos(latRAD) * Math.Cos(HArad)));
        double AzCos = (Math.Sin(DECrad) - (Math.Sin(AltRad) * Math.Sin(latRAD))) / (Math.Cos(AltRad) * Math.Cos(latRAD));
        double AzSin = -(Math.Cos(DECrad) * Math.Sin(HArad)) / Math.Cos(AltRad);
        double AzRad = Math.Atan2(AzSin, AzCos);

        if (AzRad < 0)
        {
            AzRad += 2 * Math.PI;
        }

        return new double[] {AltRad, AzRad};
    }



    public static float Ballestros(float bminv)
    {
        return (float)(4600 * (1 / (0.92 * bminv + 1.7) + 1 / (0.92 * bminv + 0.62)));
    }

    public static int[] KelvToRGB(float tempK)
    {
        var t = tempK / 100;

        int R = 0;
        int G = 0;
        int B = 0;

        if (tempK <= 6600f)
        {
            R = 255;
            G = (int)Math.Round(99.4708025861 * Math.Log(t) - 161.1195681661);
        }
        else if (tempK > 6600f)
        {
            R = (int)Math.Round(329.698727446 * Math.Pow(t - 60, -0.1332047592));
            G = (int)Math.Round(288.1221695283 * Math.Pow(t - 60, -0.0755148492));
        }

        if (tempK >= 6600f)
        {
            B = 255;
        }
        else if (tempK < 6600f)
        {
            if (tempK <= 1900f)
            {
                B = 0;
            }
            else 
            {
                B = (int)Math.Round(138.5177312231 * Math.Log(t - 10) - 305.0447927307);
            }
        }

        R = Math.Clamp(R, 0, 255);
        G = Math.Clamp(G, 0, 255);
        B = Math.Clamp(B, 0, 255);

        return new int[] {R, G, B};
    }

}
