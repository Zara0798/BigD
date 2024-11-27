using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Helicopter : Vehicle
{
    public override double Size => 5.0; // Helikopterns storlek
    public Helicopter(string licensePlate) : base(licensePlate) { }
}


    

