using System.Collections.Generic;
using System.Linq;
using LiveUML.Extensions;
using LiveUML.Models;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;

namespace LiveUML.Services
{
    public class MetadataService : IMetadataService
    {
        private readonly IOrganizationService _service;

        public MetadataService(IOrganizationService service)
        {
            _service = service;
        }

        public List<EntityMetadataModel> LoadAllEntitySummaries()
        {
            var request = new RetrieveAllEntitiesRequest
            {
                EntityFilters = EntityFilters.Entity
            };

            var response = (RetrieveAllEntitiesResponse)_service.Execute(request);

            return response.EntityMetadata
                .Where(e => e.DisplayName?.UserLocalizedLabel != null)
                .OrderBy(e => e.DisplayName.UserLocalizedLabel.Label)
                .Select(e => e.ToModel())
                .ToList();
        }

        public void LoadEntityDetail(EntityMetadataModel entity)
        {
            if (entity.IsDetailLoaded)
                return;

            var request = new RetrieveEntityRequest
            {
                LogicalName = entity.LogicalName,
                EntityFilters = EntityFilters.Attributes | EntityFilters.Relationships
            };

            var response = (RetrieveEntityResponse)_service.Execute(request);
            var metadata = response.EntityMetadata;

            entity.Attributes = metadata.Attributes
                .Where(a => a.DisplayName?.UserLocalizedLabel != null)
                .OrderByDescending(a => a.IsPrimaryId == true)
                .ThenByDescending(a => a.IsPrimaryName == true)
                .ThenBy(a => a.LogicalName)
                .Select(a => a.ToModel())
                .ToList();

            var relationships = new List<RelationshipMetadataModel>();

            if (metadata.OneToManyRelationships != null)
            {
                relationships.AddRange(metadata.OneToManyRelationships
                    .Select(r => r.ToModel(Models.RelationshipType.OneToMany)));
            }

            if (metadata.ManyToOneRelationships != null)
            {
                relationships.AddRange(metadata.ManyToOneRelationships
                    .Select(r => r.ToModel(Models.RelationshipType.ManyToOne)));
            }

            if (metadata.ManyToManyRelationships != null)
            {
                relationships.AddRange(metadata.ManyToManyRelationships
                    .Select(r => r.ToModel(entity.LogicalName)));
            }

            entity.Relationships = relationships
                .OrderBy(r => r.SchemaName)
                .ToList();

            entity.IsDetailLoaded = true;
        }
    }
}
