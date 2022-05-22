using System;

namespace Domain
{
    public class Country
    {
        public Guid Id { get; private set; }
        public string Code { get; set; }

        public Country() { }

        public Country(string code)
        {
            Code = code;
        }
    }
}
