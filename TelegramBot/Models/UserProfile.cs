namespace TelegramBot.Models
{
    public class UserProfile
    {
        public string Id { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string OrganizationId { get; set; } = string.Empty;

        public string DepartmentId { get; set; } = string.Empty;

        public string DepartmentName { get; set; } = string.Empty;

        public string TitleId { get; set; } = string.Empty;


        public string TitleName { get; set; } = string.Empty;

        public string YearsOfExperience { get; set; } = string.Empty;
    }
}
