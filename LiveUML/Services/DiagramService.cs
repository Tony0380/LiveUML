using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using LiveUML.Models;
using LiveUML.Rendering;

namespace LiveUML.Services
{
    public class DiagramService : IDiagramService
    {
        private readonly LayoutEngine _layoutEngine = new LayoutEngine();

        public DiagramLayout BuildLayout(List<EntityMetadataModel> allEntities, Dictionary<string, Point> manualPositions)
        {
            var selectedEntities = allEntities
                .Where(e => e.IsSelected)
                .ToList();

            if (selectedEntities.Count == 0)
                return new DiagramLayout();

            var boxes = new List<EntityBox>();
            foreach (var entity in selectedEntities)
            {
                var attributeLines = entity.Attributes
                    .Where(a => a.IsSelected)
                    .Select(a =>
                    {
                        string prefix = a.IsPrimaryId ? "PK " : a.IsPrimaryName ? "* " : "  ";
                        return prefix + a.LogicalName + " : " + a.DataType;
                    })
                    .ToList();

                boxes.Add(new EntityBox
                {
                    EntityLogicalName = entity.LogicalName,
                    DisplayName = entity.DisplayName,
                    AttributeLines = attributeLines
                });
            }

            // Auto-show all relationships where both entities are selected
            var selectedEntityNames = new HashSet<string>(selectedEntities.Select(e => e.LogicalName));
            var relationships = selectedEntities
                .SelectMany(e => e.Relationships)
                .Where(r =>
                {
                    var entityA = r.ReferencedEntity ?? "";
                    var entityB = r.ReferencingEntity ?? "";
                    return selectedEntityNames.Contains(entityA) && selectedEntityNames.Contains(entityB);
                })
                .ToList();

            return _layoutEngine.ComputeLayout(boxes, relationships, selectedEntities, manualPositions);
        }
    }
}
