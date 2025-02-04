using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.UseCases.NotesUseCases.Create;
using UniversiteDomain.UseCases.SecurityUseCases.Get;

namespace UniversiteRestApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NoteController(IRepositoryFactory repositoryFactory) : ControllerBase
    {
        // PUT api/<NoteController>/generate
        [HttpGet("generate/{numeroUe}")]
        public async Task<IActionResult> GenerateCsvAsync(string numeroUe)
        {
            string role = "";
            string email = "";
            IUniversiteUser user = null;
            var useCase = new CreateCSVNotesUseCase(repositoryFactory);
            try
            {
                CheckSecu(out role, out email, out user);
            }
            catch (Exception)
            {
                return Unauthorized();
            }

            if (!useCase.IsAuthorized(role)) return Unauthorized();
            
            var stream = await useCase.ExecuteAsync(numeroUe);

            return File(stream, "text/csv", $"notes_{numeroUe}.csv");
        }
        
        // PUT api/<NoteController>/5
        [HttpPut("upload/{numeroUe}")]
        public async Task<IActionResult> PutAsync(string numeroUe, IFormFile csvFile)
        {
            CreateNotesFromCsvUseCase addNotesUc = new CreateNotesFromCsvUseCase(repositoryFactory);
            
            string role="";
            string email="";
            IUniversiteUser user = null;
            try
            {
                CheckSecu(out role, out email, out user);
            }
            catch (Exception e)
            {
                return Unauthorized();
            }
            if (!addNotesUc.IsAuthorized(role)) return Unauthorized();

            byte[] fileBytes;
            using (var memoryStream = new MemoryStream())
            {
                await csvFile.CopyToAsync(memoryStream);
                fileBytes = memoryStream.ToArray();
            }

            try
            {
                await addNotesUc.ExecuteAsync(numeroUe, fileBytes);
            }
            catch (Exception e)
            {
                ModelState.AddModelError(nameof(e), e.Message);
                return ValidationProblem();
            }
            
            return NoContent();
        }
        
        
        private void CheckSecu(out string role, out string email, out IUniversiteUser user)
        {
            role = "";
            ClaimsPrincipal claims = HttpContext.User;
            if (claims.FindFirst(ClaimTypes.Email)==null) throw new UnauthorizedAccessException();
            email = claims.FindFirst(ClaimTypes.Email).Value;
            if (email==null) throw new UnauthorizedAccessException();
            //user = repositoryFactory.UniversiteUserRepository().FindByEmailAsync(email).Result;
            user = new FindUniversiteUserByEmailUseCase(repositoryFactory).ExecuteAsync(email).Result;
            if (user==null) throw new UnauthorizedAccessException();
            if (claims.Identity?.IsAuthenticated != true) throw new UnauthorizedAccessException();
            var ident = claims.Identities.FirstOrDefault();
            if (ident == null)throw new UnauthorizedAccessException();
            if (claims.FindFirst(ClaimTypes.Role)==null) throw new UnauthorizedAccessException();
            role = ident.FindFirst(ClaimTypes.Role).Value;
            if (role == null) throw new UnauthorizedAccessException();
        }
    }
}