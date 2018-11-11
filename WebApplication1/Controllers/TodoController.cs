using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    /// <summary>
    /// A Basic CRUD example of a TodoApi Controller
    /// </summary>
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class TodoController : ControllerBase
    {
        private ApplicationDatabaseContext DatabaseContext { get; }
        /// <summary>
        /// Create an instance of the TodoApi Controller and request the ApplicationDatabaseContext with dependency injection
        /// </summary>
        /// <param name="dbContext"></param>
        public TodoController(ApplicationDatabaseContext dbContext)
        {
            DatabaseContext = dbContext;
        }


        /// <summary>
        /// Get a list of all API keys from this server
        /// </summary>
        /// <returns>An array of all API keys in string format</returns>
        [HttpGet("keys")]
        [ProducesResponseType(typeof(List<TodoItem>), 200)]
        public async Task<IActionResult> GetKeys()
        {
            return Ok(await DatabaseContext.TodoItems.Select(x => x.Key).ToListAsync());
        }

        /// <summary>
        /// Get all TodoItems associated to an individual API key
        /// </summary>
        /// <param name="apiKey">The API key that hosts all of the TodoItems</param>
        /// <returns>An array of TodoItems containing the task and id</returns>
        /// <response code="200">The TodoItems were found and returned</response>
        /// <response code="400">The request was invalid</response>
        /// <response code="404">The API Key was not found</response>
        [HttpGet]
        [ProducesResponseType(typeof(List<TodoItem>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> Get([FromQuery] string apiKey)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(apiKey)) return BadRequest();
            var items = await DatabaseContext.TodoItems.Include(x => x.Tasks).FirstOrDefaultAsync(x => x.Key == apiKey);
            if (items == null) return NotFound();
            return Ok(items.Tasks);
        }

        /// <summary>
        /// Gets an individual TodoItem from its ID and API Key
        /// </summary>
        /// <param name="id">The UUID / GUID of the TodoItem</param>
        /// <param name="apiKey">The API key that hosts this TodoItem</param>
        /// <returns>An individual TodoItem containing the task and id</returns>
        /// <response code="200">The TodoItem was found and returned</response>
        /// <response code="400">The request was invalid</response>
        /// <response code="404">The TodoItem or API Key was not found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(TodoItem), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> Get(Guid id, [FromQuery] string apiKey)
        {
            // Validation
            // - Null Checks
            if (string.IsNullOrWhiteSpace(apiKey)) return BadRequest();
            if (id == Guid.Empty) return BadRequest();
            // - Length Checks
            if (apiKey.Length > 128) return BadRequest();

            var items = await DatabaseContext.TodoItems.Include(x => x.Tasks).FirstOrDefaultAsync(x => x.Key == apiKey);
            if (items?.Tasks == null) return NotFound();
            var item = items.Tasks.FirstOrDefault(x => x.Id == id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        /// <summary>
        /// Creates a new TodoItem at the provided API Key
        /// </summary>
        /// <param name="apiKey">The API key to host this TodoItem</param>
        /// <param name="task">The TodoItem model minus the ID</param>
        /// <returns>The create TodoItem and a new ID associated to it</returns>
        /// <response code="200">The TodoItem was created successfully</response>
        /// <response code="400">The request was invalid</response>
        [HttpPost]
        [ProducesResponseType(typeof(TodoItem), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Post([FromQuery] string apiKey, [FromBody] TodoItem task)
        {
            // Validation
            // - Null Checks
            if (string.IsNullOrWhiteSpace(task.Task)) return BadRequest();
            if (string.IsNullOrWhiteSpace(apiKey)) return BadRequest();

            // - Length Checks
            if (task.Task.Length > 256) return BadRequest();
            if (apiKey.Length > 128) return BadRequest();

            var items = await DatabaseContext.TodoItems.FirstOrDefaultAsync(x => x.Key == apiKey);
            if (items == null)
            {
                items = new TodoItemKeyContainer(apiKey);
                DatabaseContext.TodoItems.Add(items);
            }
            else if (await DatabaseContext.TodoItems.Where(x => x.Key == apiKey).Select(x => x.Tasks).CountAsync() >= 64)
                return BadRequest();

            if(items.Tasks == null) items.Tasks = new List<TodoItem>();

            // Create the TodoItem
            var item = new TodoItem(Guid.NewGuid(), task.Task) { Completed = task.Completed };
            items.Tasks.Add(item);

            await DatabaseContext.SaveChangesAsync();

            return Ok(item);
        }

        /// <summary>
        /// Updates a TodoItem at the provided API key and TodoItems ID
        /// </summary>
        /// <remarks>Can only update either the task content or the completed state at once time</remarks>
        /// <param name="id">The UUID / GUID of the TodoItem</param>
        /// <param name="apiKey">The API key that hosts this TodoItem</param>
        /// <param name="task">The Updated TodoItem model</param>
        /// <returns>The updated TodoItem</returns>
        /// <response code="200">The TodoItem was updated successfully</response>
        /// <response code="400">The request was invalid</response>
        /// <response code="404">The TodoItem was not found</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(TodoItem), 200)]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> Put(Guid id, [FromQuery] string apiKey, [FromBody] TodoItem task)
        {
            // Validation
            // - Null Checks
            if (string.IsNullOrWhiteSpace(apiKey)) return BadRequest();
            if (id == default(Guid)) return NotFound();

            // - Length Checks
            if (!string.IsNullOrWhiteSpace(task.Task) && task.Task.Length > 256) return BadRequest();
            if (apiKey.Length > 128) return BadRequest();

            var items = await DatabaseContext.TodoItems.Include(x => x.Tasks).FirstOrDefaultAsync(x => x.Key == apiKey);
            if (items == null) return NotFound();
            // Find the TodoItem

            var item = items.Tasks.FirstOrDefault(x => x.Id == id);
            if (item == null) return NotFound();

            // Update the TodoItem
            if (!string.IsNullOrWhiteSpace(task.Task))
                item.Task = task.Task;
            else
                item.Completed = task.Completed;

            await DatabaseContext.SaveChangesAsync();

            return Ok(item);
        }

        /// <summary>
        /// Deletes a TodoItem by ID at the provided API key
        /// </summary>
        /// <param name="id">The UUID / GUID of the TodoItem</param>
        /// <param name="apiKey">The API key that hosts the TodoItem</param>
        /// <returns>Blank</returns>
        /// <response code="200">The TodoItem was deleted successfully</response>
        /// <response code="400">The request was invalid</response>
        /// <response code="404">The TodoItem was not found</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> Delete(Guid id, [FromQuery] string apiKey)
        {
            // Validation
            // Validation
            // - Null Checks
            if (string.IsNullOrWhiteSpace(apiKey)) return BadRequest();
            // - Length Checks
            if (apiKey.Length > 128) return BadRequest();

            var items = await DatabaseContext.TodoItems.Include(x=> x.Tasks).FirstOrDefaultAsync(x => x.Key == apiKey);
            if (items == null) return NotFound();
            var item = items.Tasks.RemoveAll(x => x.Id == id);
            if (item <= 0) return NotFound();

            await DatabaseContext.SaveChangesAsync();

            return Ok();
        }
    }
}
