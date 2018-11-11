using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class TodoItemKeyContainer
    {
        [Key]
        public string Key { get; set; }

        public List<TodoItem> Tasks { get; set; }

        private TodoItemKeyContainer() { }

        public TodoItemKeyContainer(string apiKey)
        {
            this.Key = apiKey;
            this.Tasks = new List<TodoItem>();
        }
    }
}