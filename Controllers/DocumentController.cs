using GestBurOrdAPI.Data;
using GestBurOrdAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestBurOrdAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentController : ControllerBase
    {
        private readonly AppDBcontext _context;

        public DocumentController(AppDBcontext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<Documents>> CreateDocument([FromForm] int demandeId, [FromForm] DocumentTypes documentTypes, IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("Le fichier n'est pas téléchargé ou est vide.");
            }

            var demande = await _context.Demandes.FindAsync(demandeId);
            if (demande == null)
            {
                return NotFound("La demande associée n'existe pas.");
            }


            var fileName = Path.GetFileName(file.FileName);
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            var filePath = Path.Combine(uploadsFolder, fileName);

            Directory.CreateDirectory(uploadsFolder);


            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var document = new Documents
            {
                Demande = demande,
                NomFichier = fileName,
                DocumentTypes = documentTypes,
                Url = filePath
            };

            _context.Documents.Add(document);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(CreateDocument), new { id = document.Id }, document);
        }


        [HttpGet("idDemande")]
        public async Task<IActionResult> getDocumentsByid(int id)
        {
            var docs = await _context.Documents.Where(d => d.Demande.Id == id).ToListAsync();
            return Ok(docs);
        }

        /* [HttpGet("docsDemandeUser")]
         public async Task<IActionResult> GetDocumentsByDemandeAndUser(int demandeId, int userId)
         {
             var docs = await _context.Documents
                 .Where(d => d.Demande.Id == demandeId && d.Demande.UserId == userId)
                 .ToListAsync();

             if (docs == null || docs.Count == 0)
             {
                 return NotFound("No documents found for the specified DemandeId and UserId.");
             }

             return Ok(docs);
         }*/



        [HttpDelete("deleteByDemande")]
        public async Task<IActionResult> DeleteDocumentsByDemandeId()
        {
            var documents = await _context.Documents
                                          .Where(d => d.Demande.Id == 2060)
                                          .ToListAsync();

           

            _context.Documents.RemoveRange(documents);
            await _context.SaveChangesAsync();

            return Ok(new { message = $"Documents associated have been deleted." });
        }




        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAllDocumentsDemande( int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid new Demande ID.");
            }

            var documents = await _context.Documents
                .Where(d => d.Demande.Id == 2060)
                .ToListAsync();

            if (documents.Count == 0)
            {
                return NotFound("No documents found with Demande ID = 1021.");
            }

            var newDemande = await _context.Demandes.FindAsync(id);
            if (newDemande == null)
            {
                return BadRequest("New Demande not found.");
            }

            foreach (var document in documents)
            {
                document.Demande = newDemande;
            }

            await _context.SaveChangesAsync();

            return Ok("Documents updated successfully.");
        }






    }
}
