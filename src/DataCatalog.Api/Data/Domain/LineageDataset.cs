using System.Collections.Generic;

namespace DataCatalog.Api.Data.Domain
{
    public class LineageDataset : DatasetSummary
    {
        public List<LineageTransformation> SourceTransformations { get; set; } = new List<LineageTransformation>();
        public List<LineageTransformation> SinkTransformations { get; set; } = new List<LineageTransformation>();
    }
}
