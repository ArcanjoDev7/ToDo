namespace ToDo.Domain.Models
{
    public class Todo
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public User User { get; set; }

        public required Guid UserId { get; set; } 

        public required string Text { get; set; }

        public bool IsDone { get; set; } = false;
    }
}
