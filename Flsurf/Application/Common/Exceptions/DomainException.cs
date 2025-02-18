namespace Flsurf.Application.Common.Exceptions
{
    public class DomainException : Exception
    {
        public string info { get; }

        public DomainException(string information)
            : base($"Domain exception is raised, incorrect data, info: {information}") {
            info = information; 
        }
    }
}
