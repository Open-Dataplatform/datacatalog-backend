namespace DataCatalog.DatasetResourceManagement.Services.ActiveDirectory.DTO
{
    public class CreateGroupRequestDto
    {
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public bool MailEnabled { get; set; } = false;
        public string MailNickname { get; set; }
        public bool SecurityEnabled { get; set; } = true;
        public string[] Owners { get; set; }
        public string[] Members { get; set; }
        public string Visibility { get; set; } = "Public";
    }
}
