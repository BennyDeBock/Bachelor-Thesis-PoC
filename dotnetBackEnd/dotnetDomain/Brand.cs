using System;

namespace Domain
{
    public class Brand
    {
        public Guid Id { get; private set; }
        public string Name { get; set; }

        public Brand() {}

        public Brand(string name)
        {
            Name = name;
        }
    }
}
