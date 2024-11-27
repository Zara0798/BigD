using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Bus : Vehicle
{
    public override double Size => 2.0; // Bussens storlek
    public Bus(string licensePlate) : base(licensePlate) { }
}





