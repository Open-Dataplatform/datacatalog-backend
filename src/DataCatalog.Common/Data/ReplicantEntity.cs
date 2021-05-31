namespace DataCatalog.Common.Data
{
    public class ReplicantEntity: Entity
    {       
        public int Version { get; set; }
        public string OriginEnvironment { get; set; }
        public bool OriginDeleted { get; set; }
    }
}