using Backend.Services;
using Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnetBackend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class PlayerController : ControllerBase
    {
        private readonly IPlayerService playerService;

        public PlayerController(IPlayerService playerService)
        {
            this.playerService = playerService;
        }

        [HttpGet("")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public Task<IReadOnlyList<Player>> GetPlayersAsync()
        {
            return playerService.GetAllPlayers();
        }

        [HttpGet("NoCountry")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public Task<IReadOnlyList<Player>> GetPlayersWithoutCountryAsync()
        {
            return playerService.GetAllPlayersWithoutCountry();
        }

        [HttpGet("Id")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public Task<Player> GetPlayerByIdAsync(Guid id)
        {
            return playerService.GetPlayerById(id);
        }

        [HttpGet("Percentage")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public Task<Dictionary<string, double>> GetPercentageByCountryAsync()
        {
            return playerService.GetPercentageByCountry();
        }

        [HttpPost("")]
        public Task<Player> CreatePlayerAsync(PlayerDto.Create player)
        {
            try
            {
                if (player == null)
                    return null;

                return playerService.CreatePlayerAsync(player);
            } 
            catch (Exception ex)
            {
                return null;
            }
        }

        [HttpPut("")]
        public async Task<ActionResult<bool>> UpdatePlayerAsync(PlayerDto.Update player)
        {
            try
            {
                var playerToUpdate = playerService.GetPlayerById(player.Id);

                if (playerToUpdate == null)
                    return NotFound($"Player with ID {player.Id} not found");

                return await playerService.UpdatePlayerAsync(player);
            } catch (Exception ex)
            {
                return BadRequest($"Error: {ex}");
            }
        }

        [HttpDelete("")]
        public async Task<ActionResult<bool>> DeletePlayerAsync(PlayerDto.Delete player)
        {
            try
            {
                if (await playerService.DeletePlayerAsync(player))
                    return Ok(true);

                return NotFound($"Player with ID {player.Id} not found");
            } 
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}
