using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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
        /// <summary>
        /// The in memory store of all TodoItems
        /// </summary>
        public static Dictionary<string, List<TodoItem>> TodoItems { get; set; } = new Dictionary<string, List<TodoItem>>();


        /// <summary>
        /// Get a list of all API keys from this server
        /// </summary>
        /// <returns>An array of all API keys in string format</returns>
        [HttpGet("keys")]
        [ProducesResponseType(typeof(List<TodoItem>), 200)]
        public IActionResult GetKeys()
        {
            return Ok(TodoItems.Select(x => x.Key).ToList());
        }

        /// <summary>
        /// Get all TodoItems associated to an individual API key
        /// </summary>
        /// <param name="apiKey">The API key that hosts all of the TodoItems</param>
        /// <returns>An array of TodoItems containing the task and id</returns>
        /// <response code="200">The TodoItems were found and returned</response>
        /// <response code="400">The request was invalid</response>
        [HttpGet]
        [ProducesResponseType(typeof(List<TodoItem>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.BadRequest)]
        public IActionResult Get([FromQuery] string apiKey)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(apiKey)) return BadRequest();

            return Ok(GetTodoItem(apiKey));
        }

        /// <summary>
        /// Gets an individual TodoItem from its ID and API Key
        /// </summary>
        /// <param name="id">The UUID / GUID of the TodoItem</param>
        /// <param name="apiKey">The API key that hosts this TodoItem</param>
        /// <returns>An individual TodoItem containing the task and id</returns>
        /// <response code="200">The TodoItem was found and returned</response>
        /// <response code="400">The request was invalid</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(TodoItem), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.BadRequest)]
        public IActionResult Get(Guid id, [FromQuery] string apiKey)
        {
            // Validation
            // - Null Checks
            if (string.IsNullOrWhiteSpace(apiKey)) return BadRequest();
            // - Length Checks
            if (apiKey.Length > 128) return BadRequest();
            

            var item = GetTodoItem(apiKey).FirstOrDefault(x => x.Id == id);
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
        public IActionResult Post([FromQuery] string apiKey, [FromBody] TodoItem task)
        {
            // Validation
            // - Null Checks
            if (!string.IsNullOrWhiteSpace(task.Task)) return BadRequest();
            if (string.IsNullOrWhiteSpace(apiKey)) return BadRequest();

            // - Length Checks
            if (task.Task.Length > 256) return BadRequest();
            if (apiKey.Length > 128) return BadRequest();
            if (GetTodoItem(apiKey).Count > 64) return BadRequest();

            // Create the TodoItem
            var item = new TodoItem(Guid.NewGuid(), task.Task) { Completed = task.Completed };
            GetTodoItem(apiKey).Add(item);
            return Ok(item);
        }

        /// <summary>
        /// Updates a TodoItem at the provided API key and TodoItems ID
        /// </summary>
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
        public IActionResult Put(Guid id, [FromQuery] string apiKey, [FromBody] TodoItem task)
        {
            // Validation
            // - Null Checks
            if(!string.IsNullOrWhiteSpace(task.Task)) return BadRequest();
            if (string.IsNullOrWhiteSpace(apiKey)) return BadRequest();
            if (id == default(Guid)) return NotFound();

            // - Length Checks
            if (task.Task.Length > 256) return BadRequest();
            if (apiKey.Length > 128) return BadRequest();
            
            // Find the TodoItem
            var item = GetTodoItem(apiKey).FirstOrDefault(x => x.Id == id);
            if (item == null) return NotFound();

            // Update the TodoItem
            if(!string.IsNullOrWhiteSpace(task.Task))
                item.Task = task.Task;
            else
                item.Completed = task.Completed;

            return Ok(item);
        }

        /// <summary>
        /// Deletes a TodoItem by ID at the provided API key
        /// </summary>
        /// <param name="id">The UUID / GUID of the TodoItem</param>
        /// <param name="apiKey">The API key that hosts the TodoItem</param>
        /// <returns>Blank</returns>
        /// <response code="200">The TodoItem was delete successfully</response>
        /// <response code="400">The request was invalid</response>
        /// <response code="404">The TodoItem was not found</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.NotFound)]
        public IActionResult Delete(Guid id, [FromQuery] string apiKey)
        {
            // Validation
            // Validation
            // - Null Checks
            if (string.IsNullOrWhiteSpace(apiKey)) return BadRequest();
            // - Length Checks
            if (apiKey.Length > 128) return BadRequest();



            var item = GetTodoItem(apiKey).RemoveAll(x => x.Id == id);
            if (item <= 0) return NotFound();

            return Ok();
        }

        private List<TodoItem> GetTodoItem(string key)
        {
            if (!TodoItems.ContainsKey(key)) TodoItems[key] = new List<TodoItem>();
            return TodoItems[key];
        }
    }
}
