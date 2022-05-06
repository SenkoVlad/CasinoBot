namespace Casino.DAL.Exceptions
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