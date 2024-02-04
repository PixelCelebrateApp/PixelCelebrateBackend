namespace PixelCelebrateBackend.Dtos
{
    public class ViewUsersDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public int DayOfBirth { get; set; }
        public int MonthOfBirth { get; set; }
        public string UserRole { get; set; }
    }
}