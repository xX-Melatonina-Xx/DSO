using UnityEngine;
using static System.Math;

public class Planet
{
    public double a;
    public double e;
    public double i;
    public double OMEGA;
    public double omega;
    public double M0;
    public double n;
    public double RA;
    public double DEC;

    public static Planet MERCURY = new(0.387098, 0.205630, 0.122260, 0.843546, 0.508309, 3.050765, 0.001246, 4.9630, 1.0610);
    public static Planet VENUS = new(0.723332, 0.006772, 0.059248, 1.338318, 0.957352, 0.880829, 0.027962, 4.6660, 1.1600);
    public static Planet EARTH = new(1.000000, 0.0167086, 0.00000087, -0.196535, 1.993302, 6.240061, 0.0172021, 0.0000, 1.5707963);
    public static Planet MARS = new(1.523679, 0.093400, 0.032299, 0.865309, 4.999256, 0.338423, 0.009146, 5.4336, 0.9170);
    public static Planet JUPITER = new(5.20260, 0.048498, 0.022765, 1.753601, 4.780067, 0.349065, 0.001451, 4.9190, 1.1230);
    public static Planet SATURN = new(9.55491, 0.055508, 0.043405, 1.983783, 5.923507, 5.532694, 0.000584, 0.6980, 1.4660);
    public static Planet URANUS = new(19.2184, 0.046295, 0.013497, 1.291648, 1.692850, 2.482220, 0.000205, 4.1890, -0.2640);
    public static Planet NEPTUNE = new(30.1104, 0.008988, 0.030844, 2.300061, 4.769230, 4.472019, 0.000104, 5.3290, 0.7360);


    public Planet(double a, double e, double i, double OMEGA, double omega, double M0, double n, double RA, double DEC)
    {
        this.a = a;
        this.e = e;
        this.i = i;
        this.OMEGA = OMEGA;
        this.omega = omega;
        this.M0 = M0;
        this.n = n;
        this.RA = RA;
        this.DEC = DEC;
    }

    private Vector3 GetHeliocentricPosition()
    {
        double M = M0 + n * Astrotime.GetDaysFromJ2000();
        M = M % (2 * PI);

        double E = M;
        for (int i = 0; i < 4; i++)
        {
            E = E - (E - e * Sin(E) - M) / (1 - e * Cos(E));
        }

        //double v = 2 * Atan2(Sqrt(1 + e) * Sin(E / 2), Sqrt(1 - e) * Cos(E / 2));
        double v = 2 * Atan(Sqrt((1 + e) / (1 - e)) * Tan(E / 2));

        double r = a * (1 - e * Cos(E));

        float x = (float)(r * (Cos(OMEGA) * Cos(omega + v) - Sin(OMEGA) * Sin(omega + v) * Cos(i)));
        float y = (float)(r * (Sin(OMEGA) * Cos(omega + v) + Cos(OMEGA) * Sin(omega + v) * Cos(i)));
        float z = (float)(r * (Sin(omega + v) * Sin(i)));

        return new Vector3(x, y, z);
    }

    //public Vector3 GetVectorToPlanetFromEarth()
    //{
    //    //Vector3 planetPos = GetHeliocentricPosition() - EARTH.GetHeliocentricPosition();

    //    //float R = UnityEngine.Mathf.Sqrt(planetPos.x * planetPos.x + planetPos.y * planetPos.y + planetPos.z * planetPos.z);

    //    //Vector3 planetDir = new Vector3(-planetPos.x / R, planetPos.y / R, planetPos.z / R);
    //    //return planetDir;
    //}

    public Vector3 GetPlanetDir()
    {
        Vector3 vecToPlanet = GetHeliocentricPosition() - EARTH.GetHeliocentricPosition();

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
