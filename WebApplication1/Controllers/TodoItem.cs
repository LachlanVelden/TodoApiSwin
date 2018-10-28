using System;

namespace WebApplication1.Controllers
{
    public class TodoItem
    {
        public string Task { get; set; }
        public Guid Id { get; set; }
        public TodoItem(Guid id, string v)
        {
            this.Id = id;
            this.Task = v;
        }
    }
}