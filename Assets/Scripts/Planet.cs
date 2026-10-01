using System;
using UnityEngine;
using static System.Math;

public class Planet : MonoBehaviour
{
    [SerializeField] OrbitingBody planetSO;
    [SerializeField] OrbitingBody earth;

    private void Start()
    {
        Vector3 dir = GetPlanetDir();
        transform.rotation = Quaternion.LookRotation(dir);
    }
    private void FixedUpdate()
    {
        Vector3 dir = GetPlanetDir();
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 0.0065f);
    }

    private Vector3 GetHeliocentricPosition(OrbitingBody planet)
    {
        double M = planet.M0 + planet.n * Astrotime.GetDaysFromJ2000();
        M = M % (2 * PI);

        double E = M;
        for (int i = 0; i < 4; i++)
        {
            E = E - (E - planet.e * Sin(E) - M) / (1 - planet.e * Cos(E));
        }

        double v = 2 * Atan(Sqrt((1 + planet.e) / (1 - planet.e)) * Tan(E / 2));

        double r = planet.a * (1 - planet.e * Cos(E));

        float x = (float)(r * (Cos(planet.OMEGA) * Cos(planet.omega + v) - Sin(planet.OMEGA) * Sin(planet.omega + v) * Cos(planet.i)));
        float y = (float)(r * (Sin(planet.OMEGA) * Cos(planet.omega + v) + Cos(planet.OMEGA) * Sin(planet.omega + v) * Cos(planet.i)));
        float z = (float)(r * (Sin(planet.omega + v) * Sin(planet.i)));
        return new Vector3(x, y, z);
    }

    public Vector3 GetPlanetDir()
    {
        Vector3 vecToPlanet = GetHeliocentricPosition(planetSO) - GetHeliocentricPosition(earth);

        double epsilon = 0.409092804;

        double yPrim = vecToPlanet.y * Cos(epsilon) - vecToPlanet.z * Sin(epsilon);
        double zPrim = vecToPlanet.y * Sin(epsilon) + vecToPlanet.z * Cos(epsilon);

        double R = Sqrt(vecToPlanet.x * vecToPlanet.x + yPrim * yPrim + zPrim * zPrim);

        double Dec  = Asin(zPrim / R);
        double RA = Atan2(yPrim, vecToPlanet.x);

        double HA = Astrotime.GetLST() - RA;

        double Alt = Asin(Sin(Player.lat * PI / 180) * Sin(Dec) + Cos(Player.lat * PI / 180) * Cos(Dec) * Cos(HA));
        double Az = Atan2(-Sin(HA), Tan(Dec) * Cos(Player.lat * PI / 180) - Sin(Player.lat * PI / 180) * Cos(HA));

        double x = Cos(Alt) * Sin(Az);
        double y = Cos(Alt) * Cos(Az);
        double z = Sin(Alt);

        return new Vector3(-(float)y, (float)z, (float)x);
    }
}





//public static Planet MERCURY = new(0.387098, 0.205630, 0.122260, 0.843546, 0.508309, 3.050765, 0.001246, 4.9630, 1.0610);
//public static Planet VENUS = new(0.723332, 0.006772, 0.059248, 1.338318, 0.957352, 0.880829, 0.027962, 4.6660, 1.1600);
//public static Planet EARTH = new(1.000000, 0.0167086, 0.00000087, -0.196535, 1.993302, 6.240061, 0.0172021, 0.0000, 1.5707963);
//public static Planet MARS = new(1.523679, 0.093400, 0.032299, 0.865309, 4.999256, 0.338423, 0.009146, 5.4336, 0.9170);
//public static Planet JUPITER = new(5.20260, 0.048498, 0.022765, 1.753601, 4.780067, 0.349065, 0.001451, 4.9190, 1.1230);
//public static Planet SATURN = new(9.55491, 0.055508, 0.043405, 1.983783, 5.923507, 5.532694, 0.000584, 0.6980, 1.4660);
//public static Planet URANUS = new(19.2184, 0.046295, 0.013497, 1.291648, 1.692850, 2.482220, 0.000205, 4.1890, -0.2640);
//public static Planet NEPTUNE = new(30.1104, 0.008988, 0.030844, 2.300061, 4.769230, 4.472019, 0.000104, 5.3290, 0.7360);