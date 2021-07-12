namespace DataCatalog.DatasetResourceManagement.Commands.Group
{
    public class CreateGroup
    {
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public string MailNickname { get; set; }
        public string[] Owners { get; set; }
        public string[] Members { get; set; }
    }
}
