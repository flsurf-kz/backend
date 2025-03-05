using Flsurf.Domain.Files.Entities;
using Flsurf.Domain.Freelance.Entities;
using Flsurf.Domain.Freelance.Enums;
using Flsurf.Domain.User.Entities;
using Org.BouncyCastle.Asn1.Mozilla;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flsurf.Domain.Freelance.Entities
{
    // Задачи - регистрирует Старт, Паузу, Возобновление, Работу. 
    // Если в состоянии работы то просто каждый час отправляет Worksnapshot с несколькими файлами
    // АВТОМАТИЧЕСКИ
    // Вопрос а что если не отправится Снэпшот? Этот час не будет считаться как оплаченный!!!   
    // Примичане - СЧИТАТЬСЯ БУДЕТ ПО МИНУТАМ А НЕ ПО ЧАСАМ!!! и конвертироваться в часы в меньшую сторону
    // Что делать если чел остановил работу в 2:30? и открыл через минуту? это будет считаться как новая сессия и будет считаться как новый час. 
    // ! 
    public class WorkSnapshotEntity : BaseAuditableEntity
    {
        public ICollection<FileEntity> Files { get; set; } = []; 
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }  
        // тут короче необходима логика обработки
        // значит тут считается снэпшот переключении состоянии (то есть приминяется event sourcing локально) 
        // и будет считаться те снэпшоты которые в себе включали состояние InProgress 
        public WorkSnapshotStatus Status { get; set; }

        public int WorkedHours()
        {
            if (EndDate == null)
                return 0; 

            return StartDate.Hour - EndDate.Value.Hour; 
        }

        public decimal TotalEarned()
        {
            if (Contract == null)
                throw new NullReferenceException("Contract is not loaded dumbass");

            if (Contract.BudgetType != BudgetType.Hourly || Contract.CostPerHour == null)
                return -1;

            return Contract.CostPerHour.Value * WorkedHours(); 
        }

        public void StartSession()
        {
            Status = WorkSnapshotStatus.InProgress; 

        }
    }
}
