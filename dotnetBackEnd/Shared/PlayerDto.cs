using Domain;
using System;

namespace Shared
{
    public static class PlayerDto
    {
        public class Create
        {
            public int PlayerID { get; set; }
            public string Name { get; set; }
            public bool Gender { get; set; } //true for male, false for female
            public bool Active { get; set; }
            public int? Birthyear { get; set; }
            public string PlayHand { get; set; }
            public string PlayStyle { get; set; }
            public string Grip { get; set; }
            public string CountryCode { get; set; }
        }

        public class Delete
        {
            public Guid Id { get; set; }
        }

        public class Update
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
            public bool Active { get; set; }
        }
    }
}
