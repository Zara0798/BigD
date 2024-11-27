using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public abstract class Vehicle
{
    public string LicensePlate { get; set; }
    public abstract double Size { get; } // Abstrakt egenskap

    protected Vehicle(string licensePlate)
    {
        LicensePlate = licensePlate;
    }
}

