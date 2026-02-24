namespace LiveUML.Models
{
    public class AttributeMetadataModel
    {
        public string LogicalName { get; set; }
        public string DisplayName { get; set; }
        public string DataType { get; set; }
        public bool IsPrimaryId { get; set; }
        public bool IsPrimaryName { get; set; }
        public bool IsSelected { get; set; }
    }
}
