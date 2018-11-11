using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    /// <summary>
    /// A Controller allowing an administrator to manage resources in bulk
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ManagmentController : ControllerBase
    {
        /// <summary>
        /// Delete all items
        /// </summary>
        /// <param name="config">Dependency Injected Config</param>
        /// <param name="dbContext">Dependency Injected DatabaseContext</param>
        /// <param name="managementKey">The management key configured to this server</param>
        /// <returns>Blank</returns>
        [HttpDelete("clearAll")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> ClearAll([FromServices] IConfiguration config, [FromServices] ApplicationDatabaseContext dbContext, [FromQuery] string managementKey)
        {
            if (managementKey != config["MANAGEMENT_KEY"])
                return Unauthorized();
#pragma warning disable EF1000 // Possible SQL injection vulnerability. (Its not possible as its using the property names of the tables)
            // We have to use a custom SQL query here as entity framework does not support deleting large collections of items very well
            await dbContext.Database.ExecuteSqlCommandAsync(string.Format("DELETE FROM [{0}]; DELETE FROM [{1}]", nameof(dbContext.TodoItem), nameof(dbContext.TodoItems)));
#pragma warning restore EF1000 // Possible SQL injection vulnerability.
            return Ok();
        }

        /// <summary>
        /// Delete all items within an ApiKey
        /// </summary>
        /// <param name="config">Dependency Injected Config</param>
        /// <param name="dbContext">Dependency Injected DatabaseContext</param>
        /// <param name="managementKey">The management key configured to this server</param>
        /// <param name="apiKey">The API key to be deleted</param>
        /// <returns>Blank</returns>
        /// <response code="200">Successfully deleted the API Key and values</response>
        /// <response code="401">The ManagementKey is invalid</response>
        /// <response code="404">The API key was not found</response>
        [HttpDelete("clearKey")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> ClearAllWithKey([FromServices] IConfiguration config, [FromServices] ApplicationDatabaseContext dbContext, [FromQuery] string managementKey, [FromQuery] string apiKey)
        {
            if (managementKey != config["MANAGEMENT_KEY"])
                return Unauthorized();
            var items = await dbContext.TodoItems.Where(x => x.Key == apiKey).Include(x => x.Tasks).ToListAsync();
            if (items == null || items.Count <= 0) return NotFound();
            foreach (var todoItemKeyContainer in items)
            {
                dbContext.TodoItem.RemoveRange(todoItemKeyContainer.Tasks);
                await dbContext.SaveChangesAsync();
                dbContext.TodoItems.RemoveRange(items);
                await dbContext.SaveChangesAsync();
            }

            return Ok();
        }
    }
}