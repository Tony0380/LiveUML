using System.Collections.Generic;
using System.Drawing;

namespace LiveUML.Models
{
    public class DiagramLayout
    {
        public List<EntityBox> EntityBoxes { get; set; } = new List<EntityBox>();
        public List<RelationshipLine> RelationshipLines { get; set; } = new List<RelationshipLine>();
        public Size TotalSize { get; set; }
    }

    public class EntityBox
    {
        public string EntityLogicalName { get; set; }
        public Rectangle Bounds { get; set; }
        public string DisplayName { get; set; }
        public List<string> AttributeLines { get; set; } = new List<string>();
    }

    public class RelationshipLine
    {
        public Point StartPoint { get; set; }
        public Point EndPoint { get; set; }
        public string Label { get; set; }
        public RelationshipType Type { get; set; }
        public string SourceEntityName { get; set; }
        public string TargetEntityName { get; set; }
    }
}
