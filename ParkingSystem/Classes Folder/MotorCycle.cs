using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class Motorcycle : Vehicle
{
    public override double Size => 0.5; // Motorcykelns storlek
    public Motorcycle(string licensePlate) : base(licensePlate) { }
}


