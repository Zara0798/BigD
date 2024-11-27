public class Car : Vehicle
{
    public override double Size => 1.0; // Bilens storlek
    public Car(string licensePlate) : base(licensePlate) { }
}