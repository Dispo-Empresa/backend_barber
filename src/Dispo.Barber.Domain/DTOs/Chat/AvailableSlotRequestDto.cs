namespace Dispo.Barber.Domain.DTOs.Chat
{
    public class AvailableSlotRequestDto
    {
        public int Duration { get; set; }
        public long IdUser { get; set; }
        private DateTime _dateTimeSchedule;

        public DateTime DateTimeSchedule
        {
            get => _dateTimeSchedule;
            set => _dateTimeSchedule = DateTime.SpecifyKind(value, DateTimeKind.Utc).ToUniversalTime();
        }
    }
}
