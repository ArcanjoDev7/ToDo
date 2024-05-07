namespace ToDo.Domain.Requests
{
    public record EditTodoRequest
    {
        public string? Text { get; set; }
        public bool? IsDone { get; set; } = null;
    }
}
