using System.Net;

namespace Flsurf.Application.Common.cqrs
{
    public class CommandResult
    {
        public string Message { get; }
        public string? Id { get; }
        public List<string>? Ids { get; }
        public HttpStatusCode Status { get; }

        public bool IsSuccess => Status == HttpStatusCode.OK;

        public Guid? GetIdAsGuid() {
            if (Id == null)
                return null; 
            return Guid.Parse(Id); 
        } 

        private CommandResult(string message, string? id, List<string>? ids, HttpStatusCode status)
        {
            Message = message;
            Id = id;
            Ids = ids;
            Status = status;
        }


        // Успех: создание объекта
        public static CommandResult Success(string message, Guid id) =>
            new CommandResult(message, id.ToString(), null, HttpStatusCode.OK);

        // Успех: создание объекта
        public static CommandResult Success(Guid id) =>
            new CommandResult(string.Empty, id.ToString(), null, HttpStatusCode.OK);

        // Успех: без данных
        public static CommandResult Success() =>
            new CommandResult(string.Empty, null, null, HttpStatusCode.OK);

        // Успех: список объектов
        public static CommandResult Success(List<Guid> successIds) =>
            new CommandResult(string.Empty, null, successIds.Select(id => id.ToString()).ToList(), HttpStatusCode.OK);

        // Ошибка: не найдено (один объект)
        public static CommandResult NotFound(string message, Guid id) =>
            new CommandResult(message, id.ToString(), null, HttpStatusCode.NotFound);

        public static CommandResult NotFound(string message, string id)
        {
            return new CommandResult(message, id, null, HttpStatusCode.NotFound);
        }

        // Ошибка: не найдено (список объектов)
        public static CommandResult NotFound(string message, List<Guid> ids) =>
            new CommandResult(message, null, ids.Select(id => id.ToString()).ToList(), HttpStatusCode.NotFound);

        // Ошибка: плохой запрос
        public static CommandResult BadRequest(string message) =>
            new CommandResult(message, null, null, HttpStatusCode.BadRequest);

        // Ошибка: нарушение инвариантов
        public static CommandResult UnprocessableEntity(string message) =>
            new CommandResult(message, null, null, HttpStatusCode.UnprocessableEntity);

        // Ошибка: нарушение инвариантов с указанием объекта
        public static CommandResult UnprocessableEntity(string message, Guid id) =>
            new CommandResult(message, id.ToString(), null, HttpStatusCode.UnprocessableEntity);

        // Ошибка: внутренний сбой сервера
        public static CommandResult InternalServerError(string message) =>
            new CommandResult(message, null, null, HttpStatusCode.InternalServerError);

        // Ошибка: доступ запрещен
        public static CommandResult Forbidden(string message) =>
            new CommandResult(message, null, null, HttpStatusCode.Forbidden);

        // Пустой результат
        public static CommandResult Empty() =>
            new CommandResult(string.Empty, null, null, HttpStatusCode.NoContent);

        // Ошибка: конфликт данных
        public static CommandResult Conflict(string message) =>
            new CommandResult(message, null, null, HttpStatusCode.Conflict);
    }
}
