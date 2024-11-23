using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiTask.Context;
using WebApiTask.Models;

namespace WebApiTask.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TaskController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Task
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Models.Task>>> GetTasks()
        {
            var tasks = await _context.Tasks.Where(t => !t.IsDeleted).ToListAsync();
            return Ok(new { success = true, data = tasks });
        }

        // GET: api/Task/all
        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<Models.Task>>> GetAllTasks()
        {
            var tasks = await _context.Tasks.ToListAsync();
            return Ok(new { success = true, data = tasks });
        }

        // GET: api/Task/stats
        [HttpGet("stats")]
        public async Task<ActionResult<object>> GetTaskStats()
        {
            var stats = await _context.Tasks
                .GroupBy(t => new { t.IsCompleted, t.IsDeleted })
                .Select(g => new
                {
                    g.Key.IsCompleted,
                    g.Key.IsDeleted,
                    Count = g.Count()
                }).ToListAsync();

            var totalTasks = stats.Sum(s => s.Count);
            var completedTasks = stats.Where(s => s.IsCompleted && !s.IsDeleted).Sum(s => s.Count);
            var deletedTasks = stats.Where(s => s.IsDeleted).Sum(s => s.Count);
            var pendingTasks = totalTasks - completedTasks - deletedTasks;

            return Ok(new
            {
                success = true,
                data = new { Completed = completedTasks, Pending = pendingTasks, Deleted = deletedTasks }
            });
        }

        // GET: api/Task/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Models.Task>> GetTask(int id)
        {
            var task = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);
            if (task == null)
            {
                return NotFound(new { success = false, message = "Task not found or deleted." });
            }

            return Ok(new { success = true, data = task });
        }

        // PUT: api/Task/5/complete
        [HttpPut("{id}/complete")]
        public async Task<IActionResult> CompleteTask(int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null || task.IsDeleted)
            {
                return NotFound(new { success = false, message = "Task not found or deleted." });
            }

            task.IsCompleted = true;
            await _context.SaveChangesAsync();
            return NoContent();
        }

     
        // POST: api/Task
        [HttpPost]
        public async Task<ActionResult<Models.Task>> CreateTask([FromBody] Models.Task task)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(task.Name))
                {
                    return BadRequest(new { success = false, message = "Task name is required." });
                }

                task.IsDeleted = false;

                // Verifica las fechas como string
                if (string.IsNullOrWhiteSpace(task.StartDate))
                {
                    task.StartDate = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss");  // Valor por defecto
                }
                if (string.IsNullOrWhiteSpace(task.EndDate))
                {
                    // Si no se proporciona la fecha de fin, asigna un día después de la fecha de inicio
                    task.EndDate = DateTime.UtcNow.AddDays(1).ToString("yyyy-MM-ddTHH:mm:ss");  // Valor por defecto
                }

                // Aquí no es necesario convertir a DateTime, ya que se están manejando como string
                _context.Tasks.Add(task);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetTask", new { id = task.Id }, new { success = true, data = task });
            }
            catch (Exception ex)
            {
                // Captura cualquier error y devuelve un mensaje con el detalle del error
                return StatusCode(500, new { success = false, message = "Failed to add task", error = ex.Message });
            }
        }





        // DELETE: api/Task/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null)
            {
                return NotFound(new { success = false, message = "Task not found." });
            }

            task.IsDeleted = true; // Marca como eliminada
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
