using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    /// <summary>
    /// A model that binds an API key to an array of TodoItems
    /// </summary>
    public class TodoItemKeyContainer
    {
        /// <summary>
        /// The API key to identify this model
        /// </summary>
        [Key]
        public string Key { get; set; }
        /// <summary>
        /// The collection of TodoItems
        /// </summary>
        public List<TodoItem> Tasks { get; set; }

        private TodoItemKeyContainer() { }

        /// <summary>
        /// Create an instance of this TodoItemKeyContainer
        /// </summary>
        /// <param name="apiKey">The API key to identify this model by</param>
        public TodoItemKeyContainer(string apiKey)
        {
            this.Key = apiKey;
            this.Tasks = new List<TodoItem>();
        }
    }
}