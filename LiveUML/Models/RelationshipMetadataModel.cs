namespace LiveUML.Models
{
    public enum RelationshipType
    {
        OneToMany,
        ManyToOne,
        ManyToMany
    }

    public class RelationshipMetadataModel
    {
        public string SchemaName { get; set; }
        public RelationshipType Type { get; set; }
        public string ReferencedEntity { get; set; }
        public string ReferencingEntity { get; set; }
        public string ReferencingAttribute { get; set; }
        public bool IsSelected { get; set; }
    }
}
