using System;
using UnityEngine;

public class Astrotime
{
    public static double GetJD()
    {
        DateTime utc = DateTime.UtcNow;
        double day = utc.Day + utc.Hour / 24.0 + utc.Minute / 1440.0 + utc.Second / 86400.0;
        double month = utc.Month;
        int year = utc.Year;

        if (month <= 2)
        {
            year -= 1;
            month += 12;
        }

        int A = year / 100;
        int B = 2 - A + A / 4;

        double JD = Math.Floor(365.25 * (year + 4716)) + Math.Floor(30.6001 * (month + 1)) + day + B - 1524.5;
        return JD;
    }

    public static double GetT()
    {
        return (GetJD() - 2451545.0) / 36525.0;
    }

    public static double GetDaysFromJ2000()
    {
        return GetJD() - 2451545.0;
    }

    public static double GetGST()
    {
        double GST = 280.46061837 + 360.98564736629 * (GetJD() - 2451545.0);
        GST = (GST + 360) % 360;
        return GST * Math.PI / 180;
    }

    public static double GetCurrGMST()
    {
        double GMST = 18.697374558 + 24.06570982441908 * (GetJD() - 2451545.0);
        GMST = (GMST+ 24) % 24;

        return GMST;
    }

    public static double GetLST()
    {
        return GetGST() + Player.lon * Math.PI / 180;
    }
}
