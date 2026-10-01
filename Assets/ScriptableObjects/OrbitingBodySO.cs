using UnityEngine;

[CreateAssetMenu(fileName = "OrbitingBodySO", menuName = "OrbitingBodySO/OrbitingBodySO")]
public class OrbitingBody : ScriptableObject
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
}
