using Microsoft.AspNetCore.Mvc;
using CineBit.Models;
using Microsoft.AspNetCore.Authorization;

namespace CineBit.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UtenteController : ControllerBase
    {
        private readonly IRepository<Utente> _repository;

        public UtenteController(IRepository<Utente> repository)
        {
            _repository = repository;
        }


        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var utenti = await _repository.GetAllAsync();
            return Ok(utenti);
        }
    }
}