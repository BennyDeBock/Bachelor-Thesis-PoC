using System;

namespace Domain
{
    public class Player
    {
        public Guid Id { get; private set; }
        public int PlayerID { get; set; }
        public string Name { get; set; }
        public bool Gender { get; set; } //true for male, false for female
        public bool Active { get; set; }
        public int? Birthyear { get; set; }
        public string? PlayHand { get; set; }
        public string? PlayStyle { get; set; }
        public string? Grip { get; set; }
        public Country Country { get; set; }

        public Player() { }

        public Player(int playerId, string name, bool gender, Country country, bool active, int? birthyear = null, string? playhand = null, string? playstyle = null, string? grip = null)
        {
            PlayerID = playerId;
            Name = name;
            Gender = gender;
            Active = active;
            Birthyear = birthyear;
            Country = country;
            PlayHand = playhand;
            PlayStyle = playstyle;
            Grip = grip;
        }
    }
}
