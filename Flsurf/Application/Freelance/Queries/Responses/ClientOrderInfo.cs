using Flsurf.Domain.Files.Entities;

namespace Flsurf.Application.Freelance.Queries.Responses
{
    public class ClientOrderInfo
    {
        public Guid UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public FileEntity? Avatar { get; set; } = null!;
        public bool IsVerified { get; set; }

        // Статистика заказчика
        public int ActiveJobs { get; set; } // Открытые заказы
        public int ClosedJobs { get; set; } // Завершенные заказы
        public int ArbitrationJobs { get; set; } // Заказы в арбитраже

        public int ActiveContracts { get; set; } // Текущие контракты
        public int CompletedContracts { get; set; } // Выполненные контракты
        public int ArbitrationContracts { get; set; } // Контракты в арбитраже

        // Даты
        public string RegisteredAt { get; set; } = "Неизвестно";
        public string LastActiveAt { get; set; } = "Неизвестно";

        // Верификация и премиум-статус
        public bool IsPhoneVerified { get; set; }
        public bool HasPremium { get; set; }
    }

}
