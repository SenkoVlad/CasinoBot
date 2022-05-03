namespace Casino.BLL.Services.Implementation
{
    public class EntityNotFoundException : Exception
    {
        public EntityNotFoundException(string message)
        {
            Message = message;
        }

        public override string Message { get; }
    }
}