using System.Collections.Generic;
using LiveUML.Models;

namespace LiveUML.Services
{
    public interface IMetadataService
    {
        List<EntityMetadataModel> LoadAllEntitySummaries();
        void LoadEntityDetail(EntityMetadataModel entity);
    }
}
