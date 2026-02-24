using System.Collections.Generic;

namespace LiveUML.Models
{
    public class EntityMetadataModel
    {
        public string LogicalName { get; set; }
        public string DisplayName { get; set; }
        public string SchemaName { get; set; }
        public List<AttributeMetadataModel> Attributes { get; set; } = new List<AttributeMetadataModel>();
        public List<RelationshipMetadataModel> Relationships { get; set; } = new List<RelationshipMetadataModel>();
        public bool IsDetailLoaded { get; set; }
        public bool IsSelected { get; set; }
    }
}
