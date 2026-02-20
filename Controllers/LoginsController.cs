namespace Evaluation_Nuxiba.Controllers
{
    using Evaluation_Nuxiba.Data;
    using Evaluation_Nuxiba.Models;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using System.Text;

    [Route("logins")]
    [ApiController]
    public class LoginsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public LoginsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CcLogLogin>>> GetLogins()
        {
            return await _context.CcLogLogins
                .AsNoTracking()
                .ToListAsync();
        }

        [HttpPost]
        public async Task<IActionResult> CreateLogin(CcLogLogin login)
        {
            if (login.fecha > DateTime.Now)
                return BadRequest("La fecha no puede ser futura");

            var userExists = await _context.CcUsers
                .AnyAsync(u => u.User_id == login.User_id);

            if (!userExists)
                return NotFound("El usuario no existe");

            var lastMov = await _context.CcLogLogins
                .AsNoTracking()
                .Where(l => l.User_id == login.User_id)
                .OrderByDescending(l => l.fecha)
                .FirstOrDefaultAsync();

            if (lastMov != null)
            {
                if (lastMov.TipoMov == 1 && login.TipoMov == 1)
                    return Conflict("No puedes registrar LOGIN sin LOGOUT previo");

                if (lastMov.TipoMov == 2 && login.TipoMov == 2)
                    return Conflict("No puedes registrar LOGOUT sin LOGIN previo");
            }

            try
            {
                login.fecha = DateTime.Now;

                _context.ChangeTracker.Clear();
                _context.CcLogLogins.Add(login);

                await _context.SaveChangesAsync();

                return Created("", login);
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, "Error interno al registrar movimiento");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLogin(int id, CcLogLogin login)
        {
            if (id != login.LoginId)
                return BadRequest("El LoginId no coincide");

            _context.Entry(login).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                var exists = await _context.CcLogLogins
                    .AnyAsync(e => e.LoginId == id);

                if (!exists)
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLogin(int id)
        {
            var login = await _context.CcLogLogins
                .FirstOrDefaultAsync(l => l.LoginId == id);

            if (login == null)
                return NotFound();

            _context.CcLogLogins.Remove(login);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("reporte-csv")]
        public async Task<IActionResult> GenerarCsvHorasTrabajadas()
        {
            var logins = await _context.CcLogLogins
                .Where(l => l.TipoMov == 1 && l.fecha.HasValue)
                .OrderBy(l => l.User_id)
                .ThenBy(l => l.fecha)
                .ToListAsync();

            var logouts = await _context.CcLogLogins
                .Where(l => l.TipoMov == 0 && l.fecha.HasValue)
                .OrderBy(l => l.User_id)
                .ThenBy(l => l.fecha)
                .ToListAsync();

            var sesiones = logins.Select(l =>
            {
                var logout = logouts
                    .FirstOrDefault(lo => lo.User_id == l.User_id && lo.fecha > l.fecha);

                if (logout == null) return null;

                var duracionHoras = (logout.fecha.Value - l.fecha.Value).TotalHours;
                return new { l.User_id, DuracionHoras = duracionHoras };
            })
            .Where(x => x != null)
            .ToList();

            var totalHoras = sesiones
                .GroupBy(s => s.User_id)
                .Select(g => new
                {
                    User_id = g.Key,
                    Horas = g.Sum(s => s.DuracionHoras)
                })
                .ToList();

            var usuarios = await _context.CcUsers
                .Join(_context.ccRIACat_Areas,
                      u => u.Area_id,
                      a => a.IDArea,
                      (u, a) => new
                      {
                          u.User_id,
                          Login = u.Login ?? "",
                          Nombres = u.Nombres ?? "",
                          ApellidoPaterno = u.ApellidoPaterno ?? "",
                          ApellidoMaterno = u.ApellidoMaterno ?? "",
                          AreaName = a.AreaName ?? ""
                      })
                .ToListAsync();

            var resultado = usuarios
                .Join(totalHoras,
                      u => u.User_id,
                      h => h.User_id,
                      (u, h) => new
                      {
                          u.Login,
                          NombreCompleto = (u.Nombres ?? "") + " " + (u.ApellidoPaterno ?? "") + " " + (u.ApellidoMaterno ?? ""),
                          u.AreaName,
                          HorasTrabajadas = h.Horas
                      })
                .ToList();

            var csv = new StringBuilder();
            csv.AppendLine("Login,Nombre Completo,Area,Horas Trabajadas");

            foreach (var item in resultado)
            {
                csv.AppendLine($"{item.Login},{item.NombreCompleto},{item.AreaName},{item.HorasTrabajadas:F2}");
            }

            var bytes = Encoding.UTF8.GetBytes(csv.ToString());
            return File(bytes, "text/csv", "ReporteHorasTrabajadas.csv");
        }
    }
}