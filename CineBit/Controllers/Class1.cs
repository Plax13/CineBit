using Microsoft.AspNetCore.Mvc;
using CineBit.Models;

namespace CineBit.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UtentiController : ControllerBase
    {
        private readonly IRepository<Utenti> _repository;

        public UtentiController(IRepository<Utenti> repository)
        {
            _repository = repository;
        }

        // GET: api/utenti
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var utenti = await _repository.GetAllAsync();
            return Ok(utenti);
        }
    }
}