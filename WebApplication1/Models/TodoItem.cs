using System;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    /// <summary>
    /// A uniquely identifiable model describing a task and if it has been completed
    /// </summary>
    public class TodoItem
    {
        /// <summary>
        /// The description of this Task.
        /// </summary>
        [MaxLength(256)]
        public string Task { get; set; }
        /// <summary>
        /// The Id of this TodoItem used to uniquely identify this TodoItem from others.
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// Whether or not this TodoItem has been completed
        /// </summary>
        public bool Completed { get; set; }
        /// <summary>
        /// Create a new TodoItem
        /// </summary>
        /// <param name="id"></param>
        /// <param name="v"></param>
        public TodoItem(Guid id, string v)
        {
            this.Id = id;
            this.Task = v;
            this.Completed = false;
        }

        private TodoItem()
        {
            
        }
    }
}