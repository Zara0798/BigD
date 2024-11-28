using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public abstract class Vehicle
{
    public string LicensePlate { get; set; }
    public abstract double Size { get; } // Abstrakt egenskap
    //public DateTime Arrival { get; set; } = DateTime.Now;
    public int PricePerHour { get; set; }

    public Vehicle(string licensePlate)
    {
        LicensePlate = licensePlate;
    }

    // TODO: vi kommer att behöva en ToString() för att skriva ut fordon
    public override string ToString()
    {
        return $"Vehicle: {LicensePlate}";
    }

}

