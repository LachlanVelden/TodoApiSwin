using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class TodoController : ControllerBase
    {
        public static Dictionary<string, List<TodoItem>> TodoItems { get; set; } = new Dictionary<string, List<TodoItem>>();

        [HttpGet("keys")]
        [ProducesResponseType(typeof(List<TodoItem>), 200)]
        public IActionResult GetKeys()
        {
            return Ok(TodoItems.Select(x => x.Key).ToList());
        }

        // GET api/values
        [HttpGet]
        [ProducesResponseType(typeof(List<TodoItem>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.BadRequest)]
        public IActionResult Get([FromQuery] string apiKey)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(apiKey)) return BadRequest();

            return Ok(GetTodoItem(apiKey));
        }

        // GET api/values/5
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(TodoItem), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.BadRequest)]
        public IActionResult Get(Guid id, [FromQuery] string apiKey)
        {
            // Validation
            if (apiKey.Length > 128) return BadRequest();
            if (string.IsNullOrWhiteSpace(apiKey)) return BadRequest();

            var item = GetTodoItem(apiKey).FirstOrDefault(x => x.Id == id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        // POST api/values
        [HttpPost]
        [ProducesResponseType(typeof(TodoItem), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.BadRequest)]
        public IActionResult Post([FromQuery] string apiKey, [FromBody] TodoItem task)
        {
            // Validation
            if (apiKey.Length > 128 || task.Task.Length > 256) return BadRequest();
            if (string.IsNullOrWhiteSpace(apiKey)) return BadRequest();
            if (GetTodoItem(apiKey).Count > 64) return BadRequest();

            var item = new TodoItem(Guid.NewGuid(), task.Task);
            GetTodoItem(apiKey).Add(item);
            return Ok(item);
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(TodoItem), 200)]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.NotFound)]
        public IActionResult Put(Guid id, [FromQuery] string apiKey, [FromBody] TodoItem task)
        {
            // Validation
            if (apiKey.Length > 128 || task.Task.Length > 256) return BadRequest();
            if (string.IsNullOrWhiteSpace(apiKey)) return BadRequest();
            if (id == default(Guid)) return NotFound();

            var item = GetTodoItem(apiKey).FirstOrDefault(x => x.Id == id);
            if (item == null) return NotFound();
            item.Task = task.Task;
            return Ok(item);
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.NotFound)]
        public IActionResult Delete(Guid id, [FromQuery] string apiKey)
        {
            // Validation
            if (apiKey.Length > 128) return BadRequest();
            if (string.IsNullOrWhiteSpace(apiKey)) return BadRequest();



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
