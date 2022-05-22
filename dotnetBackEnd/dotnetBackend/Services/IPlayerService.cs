using Domain;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Services
{
    public interface IPlayerService
    {
        //GET
        Task<IReadOnlyList<Player>> GetAllPlayers();
        Task<IReadOnlyList<Player>> GetAllPlayersWithoutCountry();
        Task<Player> GetPlayerById(Guid id);
        Task<Dictionary<string, double>> GetPercentageByCountry();

        //POST
        Task<Player> CreatePlayerAsync(PlayerDto.Create player);

        //PUT
        Task<bool> UpdatePlayerAsync(PlayerDto.Update player);

        //DELETE
        Task<bool> DeletePlayerAsync(PlayerDto.Delete player);
    }
}
