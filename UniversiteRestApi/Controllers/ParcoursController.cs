using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Dtos;
using UniversiteDomain.Entities;
using UniversiteDomain.UseCases.EtudiantUseCases.Get;
using UniversiteDomain.UseCases.ParcoursUseCases.Create;
using UniversiteDomain.UseCases.ParcoursUseCases.Delete;
using UniversiteDomain.UseCases.ParcoursUseCases.Get;
using UniversiteDomain.UseCases.ParcoursUseCases.Update;
using UniversiteDomain.UseCases.SecurityUseCases.Get;

namespace UniversiteRestApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ParcoursController(IRepositoryFactory repositoryFactory) : ControllerBase
    {
        // GET: api/<ParcoursController>
        [HttpGet]
        public async Task<ActionResult<List<ParcoursDto>>> GetAsync()
        {
            string role="";
            string email="";
            IUniversiteUser user=null;
            try
            {
                CheckSecu(out role, out email, out user);
            }
            catch (Exception e)
            {
                return Unauthorized();
            }
            
            GetTousLesParcoursUseCase uc = new GetTousLesParcoursUseCase(repositoryFactory);
            if (!uc.IsAuthorized(role)) return Unauthorized();
            List<Parcours> parc=null;
            try
            {
                parc = await uc.ExecuteAsync();
            }
            catch (Exception e)
            {
                return ValidationProblem();
            }
            return ParcoursDto.ToDtos(parc);
        }
        
        // GET api/<ParcoursController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ParcoursDto>>  GetUnParcours(long id)
        {
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
            
            GetParcoursByIdUseCase uc = new GetParcoursByIdUseCase(repositoryFactory);
            // On vérifie si l'utilisateur connecté a le droit d'accéder à la ressource
            if (!uc.IsAuthorized(role, user, id)) return Unauthorized();
            Parcours? parc;
            try
            {
                parc = await uc.ExecuteAsync(id);
            }
            catch (Exception e)
            {
                ModelState.AddModelError(nameof(e), e.Message);
                return ValidationProblem();
            }
           
            if (parc == null) return NotFound();
            
            return new ParcoursDto().ToDto(parc);
        }
        
        // GET api/<ParcoursController>/complet/5
        [HttpGet("complet/{id}")]
        public async Task<ActionResult<ParcoursCompletDto>> GetUnEtudiantCompletAsync(long id)
        {
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
            
            GetParcoursCompletUseCase uc = new GetParcoursCompletUseCase(repositoryFactory);

            // On vérifie si l'utilisateur connecté a le droit d'accéder à la ressource
            //if (!uc.IsAuthorized(role, user, id)) return Unauthorized();
            Parcours? parc;
            try
            {
                parc = await uc.ExecuteAsync(id);
            }
            catch (Exception e)
            {
                return ValidationProblem();
            }
            if (parc == null) return NotFound();
            return new ParcoursCompletDto().ToDto(parc);
        }
        
        // POST api/<ParcoursController>
        [HttpPost]
        public async Task<ActionResult<ParcoursDto>> PostAsync([FromBody] ParcoursDto parcoursDto)
        {
            CreateParcoursUseCase createParcoursUc = new CreateParcoursUseCase(repositoryFactory);

            string role="";
            string email="";
            IUniversiteUser user = null;
            CheckSecu(out role, out email, out user);
            if (!createParcoursUc.IsAuthorized(role)) return Unauthorized();
            
            Parcours parc = parcoursDto.ToEntity();
            
            try
            {
                parc = await createParcoursUc.ExecuteAsync(parc);
            }
            catch (Exception e)
            {
                ModelState.AddModelError(nameof(e), e.Message);
                return ValidationProblem();
            }

            ParcoursDto dto = new ParcoursDto().ToDto(parc);
            return CreatedAtAction(nameof(GetUnParcours), new { id = dto.Id }, dto);
        }
        
        // PUT api/<ParcoursController>/5
        [HttpPut("{id}")]
        public async Task<ActionResult<ParcoursDto>> PutAsync(long id, [FromBody] ParcoursDto parcoursDto)
        {
            UpdateParcoursUseCase updateParcoursUc = new UpdateParcoursUseCase(repositoryFactory);

            if (id != parcoursDto.Id)
            {
                return BadRequest();
            }
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
            if (!updateParcoursUc.IsAuthorized(role)) return Unauthorized();
            // Mise à jour de l'étudiant
            try
            {
                await updateParcoursUc.ExecuteAsync(parcoursDto.ToEntity());
            }
            catch (Exception e)
            {
                ModelState.AddModelError(nameof(e), e.Message);
                return ValidationProblem();
            }
            
            return NoContent();
        }

        // DELETE api/<ParcoursController>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Parcours>> DeleteAsync(long id)
        {
            DeleteParcoursUseCase parcoursUc = new DeleteParcoursUseCase(repositoryFactory);
            string role = "";
            string email = "";
            IUniversiteUser user = null;
            try
            {
                CheckSecu(out role, out email, out user);
            }
            catch (Exception e)
            {
                return Unauthorized();
            }

            if (!parcoursUc.IsAuthorized(role)) return Unauthorized();
            // On supprime l'étudiant et le user avec l'Id id
            try
            {
                await parcoursUc.ExecuteAsync(id);
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