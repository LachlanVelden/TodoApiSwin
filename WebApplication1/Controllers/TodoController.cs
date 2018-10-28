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
        // GET api/values
        [HttpGet]
        [ProducesResponseType(typeof(Dictionary<string, List<TodoItem>>), 200)]
        public IActionResult Get([FromQuery] string apiKey)
        {
            if(string.IsNullOrWhiteSpace(apiKey)) return BadRequest();
            if(!TodoItems.ContainsKey(apiKey)) TodoItems[apiKey] = new List<TodoItem>
            {
                new TodoItem(Guid.NewGuid(), "Complete the task lochie has given you...")
            };
            return Ok(TodoItems[apiKey]);
        }

        // GET api/values/5
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(TodoItem), 200)]
        public IActionResult Get(Guid id, [FromQuery] string apiKey)
        {
            if (string.IsNullOrWhiteSpace(apiKey)) return BadRequest();
            if (!TodoItems.ContainsKey(apiKey)) TodoItems[apiKey] = new List<TodoItem>
            {
                new TodoItem(Guid.NewGuid(), "Complete the task lochie has given you...")
            };
            var item = TodoItems[apiKey].FirstOrDefault(x => x.Id == id);
            if(item == null) return NotFound();
            return Ok(item);
        }

        // POST api/values
        [HttpPost]
        [ProducesResponseType(typeof(TodoItem), 200)]
        public IActionResult Post([FromQuery] string apiKey, [FromBody] TodoItem task)
        {
            if (string.IsNullOrWhiteSpace(apiKey)) return BadRequest();
            if (!TodoItems.ContainsKey(apiKey)) TodoItems[apiKey] = new List<TodoItem>
            {
                new TodoItem(Guid.NewGuid(), "Complete the task lochie has given you...")
            };
            var item = new TodoItem(Guid.NewGuid(), task.Task);
            TodoItems[apiKey].Add(item);
            return Ok(item);
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(TodoItem), 200)]
        public IActionResult Put(Guid id, [FromQuery] string apiKey, [FromBody] TodoItem task)
        {
            if (string.IsNullOrWhiteSpace(apiKey)) return BadRequest();
            if (!TodoItems.ContainsKey(apiKey)) TodoItems[apiKey] = new List<TodoItem>
            {
                new TodoItem(Guid.NewGuid(), "Complete the task lochie has given you...")
            };
            var item = TodoItems[apiKey].FirstOrDefault(x=> x.Id == id);
            if(id == null) return NotFound();
            item.Task = task.Task;
            return Ok(item);
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id, [FromQuery] string apiKey)
        {
            if (string.IsNullOrWhiteSpace(apiKey)) return BadRequest();
            if (!TodoItems.ContainsKey(apiKey)) TodoItems[apiKey] = new List<TodoItem>
            {
                new TodoItem(Guid.NewGuid(), "Complete the task lochie has given you...")
            };
            var item = TodoItems[apiKey].RemoveAll(x => x.Id == id);
            if (item <= 0) return NotFound();
            
            return Ok();
        }
    }
}
