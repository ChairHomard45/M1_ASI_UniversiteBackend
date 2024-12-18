using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Dtos;
using UniversiteDomain.Entities;
using UniversiteDomain.UseCases.SecurityUseCases.Get;
using UniversiteDomain.UseCases.UeUseCases.Create;
using UniversiteDomain.UseCases.UeUseCases.Delete;
using UniversiteDomain.UseCases.UeUseCases.Get;
using UniversiteDomain.UseCases.UeUseCases.Update;

namespace UniversiteRestApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UEController(IRepositoryFactory repositoryFactory) : ControllerBase
    {
        // GET: api/<UEController>
        [HttpGet]
        public async Task<ActionResult<List<UeDto>>> GetAsync()
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
            
            GetToutesLesUesUseCase uc = new GetToutesLesUesUseCase(repositoryFactory);
            if (!uc.IsAuthorized(role)) return Unauthorized();
            List<Ue> etud=null;
            try
            {
                etud = await uc.ExecuteAsync();
            }
            catch (Exception e)
            {
                return ValidationProblem();
            }
            return UeDto.ToDtos(etud);
        }
        
        // GET api/<UEController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UeDto>>  GetUneUe(long id)
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
            
            GetUeByIdUseCase uc = new GetUeByIdUseCase(repositoryFactory);
            // On vérifie si l'utilisateur connecté a le droit d'accéder à la ressource
            if (!uc.IsAuthorized(role, user, id)) return Unauthorized();
            Ue? ue;
            try
            {
                ue = await uc.ExecuteAsync(id);
            }
            catch (Exception e)
            {
                ModelState.AddModelError(nameof(e), e.Message);
                return ValidationProblem();
            }
           
            if (ue == null) return NotFound();
            
            return new UeDto().ToDto(ue);
        }
        
        // GET api/<UEController>/complet/5
        [HttpGet("complet/{id}")]
        public async Task<ActionResult<UeCompletDto>> GetUneUeCompletAsync(long id)
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
            
            GetUeCompletUseCase uc = new GetUeCompletUseCase(repositoryFactory);

            // On vérifie si l'utilisateur connecté a le droit d'accéder à la ressource
            //if (!uc.IsAuthorized(role, user, id)) return Unauthorized();
            Ue? ue;
            try
            {
                ue = await uc.ExecuteAsync(id);
            }
            catch (Exception e)
            {
                return ValidationProblem();
            }
            if (ue == null) return NotFound();
            return new UeCompletDto().ToDto(ue);
        }
        
        
        // POST api/<UEController>
        [HttpPost]
        public async Task<ActionResult<EtudiantDto>> PostAsync([FromBody] UeDto ueDto)
        {
            CreateUeUseCase createUeUc = new CreateUeUseCase(repositoryFactory);
            string role="";
            string email="";
            IUniversiteUser user = null;
            CheckSecu(out role, out email, out user);
            if (!createUeUc.IsAuthorized(role)) return Unauthorized();
            
            Ue ue = ueDto.ToEntity();
            
            try
            {
                ue = await createUeUc.ExecuteAsync(ue);
            }
            catch (Exception e)
            {
                ModelState.AddModelError(nameof(e), e.Message);
                return ValidationProblem();
            }

            UeDto dto = new UeDto().ToDto(ue);
            return CreatedAtAction(nameof(GetUneUe), new { id = dto.Id }, dto);
        }
        
        
        // PUT api/<UeController>/5
        [HttpPut("{id}")]
        public async Task<ActionResult<EtudiantDto>> PutAsync(long id, [FromBody] UeDto ueDto)
        {
            UpdateUeUseCase updateUetUc = new UpdateUeUseCase(repositoryFactory);
            
            if (id != ueDto.Id)
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
            if (!updateUetUc.IsAuthorized(role)) return Unauthorized();
            // Mise à jour de l'étudiant
            try
            {
                await updateUetUc.ExecuteAsync(ueDto.ToEntity());
            }
            catch (Exception e)
            {
                ModelState.AddModelError(nameof(e), e.Message);
                return ValidationProblem();
            }
            
            return NoContent();
        }
        
        // DELETE api/<UeController>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Parcours>> DeleteAsync(long id)
        {
            DeleteUeUseCase ueUc = new DeleteUeUseCase(repositoryFactory);
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

            if (!ueUc.IsAuthorized(role)) return Unauthorized();
            // On supprime l'ue
            try
            {
                await ueUc.ExecuteAsync(id);
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