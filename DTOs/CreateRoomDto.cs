using System.ComponentModel.DataAnnotations;

namespace RoomBooking.Api.DTOs
{
    public class CreateRoomDto
    {
        [Required(ErrorMessage = "Название комнаты обязательно")]
        [StringLength(100, ErrorMessage = "Имя не может быть длиннее 100 символов")]
        public string Name { get; set; } = string.Empty;

        [Range(1, 500, ErrorMessage = "Вместимость должна быть от 0 до 500 человек")]
        public int Capacity {  get; set; }

        public string Description { get; set; } = string.Empty;
    }
}
