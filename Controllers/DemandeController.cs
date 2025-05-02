using GestBurOrdAPI.Data;
using GestBurOrdAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestBurOrdAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DemandeController : ControllerBase
    {
        private readonly AppDBcontext _context;

        public DemandeController(AppDBcontext context)
        {
            _context = context;
        }



        [HttpPost]
        public async Task<ActionResult> CreateDemande([FromBody] Demande demande)
        {
            if (demande == null)
            {
                return BadRequest("La demande ne peut pas être nulle.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingService = await _context.Services.FindAsync(demande.ServiceId);
            if (existingService == null)
            {
                return BadRequest($"Le service avec ID {demande.ServiceId} n'existe pas.");
            }

            demande.Service = null;

            _context.Demandes.Add(demande);

            if (demande.Documents != null && demande.Documents.Any())
            {
                foreach (var document in demande.Documents)
                {
                    _context.Documents.Add(document);
                }
            }

            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(CreateDemande), new { id = demande.Id }, demande);
        }


        [HttpGet("allDemandes")]
        public async Task<IActionResult> getAllDemandes()
        {


            var demandes = await _context.Demandes.ToListAsync();
            return Ok(demandes);

        }


        [HttpGet("nbDemandes")]
        public async Task<IActionResult> getNBDemandes()
        {
            var nbDemandes = await _context.Demandes.CountAsync();
            return Ok(nbDemandes);
        }

        [HttpGet("nbStatusDemandes")]
        public async Task<IActionResult> getStatusDemandes(int status)
        {
            Statuses st;

            if (status == 0)
             st= Statuses.EnAttente;
            else if(status == 1)
                st = Statuses.EnCours;
            else if (status == 2)
                st = Statuses.Traite;
            else
                st = Statuses.Refuse;



            var demandes = 0;
             demandes = await _context.Demandes.Where(d => d.Statut == st).CountAsync();
            

            return Ok(demandes);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult> GetDemandeById(int id)
        {
            var demande = await _context.Demandes
        .Include(d => d.Documents)
        .FirstOrDefaultAsync(d => d.Id == id);

            if (demande == null)
            {
                return NotFound();
            }

            return Ok(demande);
        }

        [HttpGet("demandeUserId/{id}")]
        public async Task<ActionResult> GetDemandesByUserId(int id)
        {
            var demandes = await _context.Demandes
                .Include(d => d.Documents)
                .Where(d => d.UserId == id)
                .ToListAsync();

            if (!demandes.Any())
            {
                return NotFound();
            }

            return Ok(demandes);
        }
        [HttpGet("nbStatusDmdUserId/{id}")]
        public async Task<IActionResult> GetStatusDmdUserId(int status, int id)
        {
            Statuses st;

            if (status == 0)
                st = Statuses.EnAttente;
            else if (status == 1)
                st = Statuses.EnCours;
            else if (status == 2)
                st = Statuses.Traite;
            else
                st = Statuses.Refuse;

            var demandes = await _context.Demandes
                .Where(d => d.Statut == st && d.UserId == id)
                .CountAsync();

            return Ok(demandes);
        }

        [HttpPut("updateStatus/{id}")]
        public async Task<IActionResult> updateStatus(int id, int status)
        {
            Statuses newStatus;
            switch (status)
            {
                case 0:
                    newStatus = Statuses.EnAttente;
                    break;
                case 1:
                    newStatus = Statuses.EnCours;
                    break;
                case 2:
                    newStatus = Statuses.Traite;
                    break;
                case 3:
                    newStatus = Statuses.Refuse;
                    break;
                default:
                    return BadRequest("Invalid status value.");
            }

            var demande = await _context.Demandes.FindAsync(id);
            if (demande == null)
            {
                return NotFound($"Demande with ID {id} not found.");
            }

            demande.Statut = newStatus;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"Error updating demande status: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while updating the status.");
            }

            return Ok($"Status of demande with ID {id} updated to {newStatus}.");
        }





    }



}
