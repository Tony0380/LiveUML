using System.Collections.Generic;
using System.Drawing;
using LiveUML.Models;

namespace LiveUML.Services
{
    public interface IDiagramService
    {
        DiagramLayout BuildLayout(List<EntityMetadataModel> allEntities, Dictionary<string, Point> manualPositions);
    }
}
