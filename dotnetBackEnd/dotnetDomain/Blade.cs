using System;

namespace Domain
{
    public class Blade
    {
        public Guid Id { get; private set; }
        public string Name { get; set; }
        public double? Speed { get; set; }
        public double? Control { get; set; }
        public double? Stiffness { get; set; }
        public double? Overall { get; set; }
        public double? Price { get; set; }
        public int Ratings { get; set; }
        public Brand Brand { get; set; }

        public Blade() { }

        public Blade(string name, Brand brand, double? speed = null, double? control = null, double? stiffness = null, double? overall = null, double? price = null, int ratings = 0)
        {
            Name = name;
            Speed = speed;
            Control = control;
            Stiffness = stiffness;
            Overall = overall;
            Price = price;
            Ratings = ratings;
            Brand = brand;
        }
    }
}
