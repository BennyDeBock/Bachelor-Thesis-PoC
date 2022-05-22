using Backend.Data;
using Domain;
using Microsoft.EntityFrameworkCore;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Services
{
    public class PlayerService : IPlayerService
    {
        private readonly DbSet<Player> players;
        private readonly DbSet<Country> countries;
        private readonly Context context;

        public PlayerService(Context context)
        {
            this.context = context;
            players = context.Players;
            countries = context.Countries;
        }

        public async Task<Player> CreatePlayerAsync(PlayerDto.Create player)
        {
            if (player == null)
                return null;

            var exists = await players
                .Include(p => p.Country)
                .SingleOrDefaultAsync(p =>
                    p.Name == player.Name &&
                    p.Country.Code == player.CountryCode
                ) == null ? false : true;

            if(!exists)
            {
                var country = await countries.SingleOrDefaultAsync(c => c.Code == player.CountryCode);
                var newPlayer = new Player(
                    player.PlayerID, 
                    player.Name, 
                    player.Gender,
                    country,
                    player.Active,
                    player.Birthyear,
                    player.PlayHand,
                    player.PlayStyle,
                    player.Grip
                );
                if (country == null)
                {
                    var newCountry = new Country(player.CountryCode);
                    newPlayer.Country = newCountry;
                    await countries.AddAsync(newCountry);
                }
                else
                    newPlayer.Country = country;
                await players.AddAsync(newPlayer);
            }

            await context.SaveChangesAsync();

            return await players
                .Include(p => p.Country)
                .SingleOrDefaultAsync(p => p.Name == player.Name && p.Country.Code == player.CountryCode);
        }

        public async Task<bool> DeletePlayerAsync(PlayerDto.Delete player)
        {
            var exist = await players.SingleOrDefaultAsync(p => p.Id == player.Id);

            if (exist == null)
                return false;

            players.Remove(exist);

            return await context.SaveChangesAsync() > 0;
        }

        public async Task<IReadOnlyList<Player>> GetAllPlayers()
        {
            return await players
                .Include(p => p.Country)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IReadOnlyList<Player>> GetAllPlayersWithoutCountry()
        {
            return await players
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Player> GetPlayerById(Guid id)
        {
            return await players
                .Include(p => p.Country)
                .SingleOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Dictionary<string, double>> GetPercentageByCountry()
        {
            Dictionary<string, double> percentages = new();
            /*var query = players
                .Include(p => p.Country)
                .GroupBy(
                    p => p.Country.Code,
                    p => p,
                    (country, player) => new
                    {
                        Key = country,
                        Count = player.Count()
                    }
                );*/
            var query = players
                .Include(p => p.Country)
                .GroupBy(
                    p => new { p.Country.Code }
                )
                .Select(g => new
                {
                    Country = g.Key.Code,
                    Count = (double)g.Count() / players.Count() * 100
                }).OrderByDescending(x => x.Count);

            foreach (var result in query)
            {
                percentages.Add(result.Country, result.Count);
            }

            return percentages;
        }

        public async Task<bool> UpdatePlayerAsync(PlayerDto.Update player)
        {
            if (player == null)
                return false;

            var playerToUpdate = await players.Include(p => p.Country)
                .SingleOrDefaultAsync(p => p.Id == player.Id);

            if (playerToUpdate != null)
            {
                playerToUpdate.Name = player.Name;
                playerToUpdate.Active = player.Active;
            }

            return await context.SaveChangesAsync() > 0;
        }
    }
}
