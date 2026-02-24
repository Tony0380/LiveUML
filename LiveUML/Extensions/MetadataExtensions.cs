using System.Linq;
using LiveUML.Models;
using Microsoft.Xrm.Sdk.Metadata;

namespace LiveUML.Extensions
{
    public static class MetadataExtensions
    {
        public static EntityMetadataModel ToModel(this EntityMetadata entity)
        {
            return new EntityMetadataModel
            {
                LogicalName = entity.LogicalName,
                DisplayName = entity.DisplayName?.UserLocalizedLabel?.Label ?? entity.LogicalName,
                SchemaName = entity.SchemaName
            };
        }

        public static AttributeMetadataModel ToModel(this AttributeMetadata attribute)
        {
            return new AttributeMetadataModel
            {
                LogicalName = attribute.LogicalName,
                DisplayName = attribute.DisplayName?.UserLocalizedLabel?.Label ?? attribute.LogicalName,
                DataType = attribute.AttributeTypeName?.Value ?? attribute.AttributeType?.ToString() ?? "Unknown",
                IsPrimaryId = attribute.IsPrimaryId == true,
                IsPrimaryName = attribute.IsPrimaryName == true
            };
        }

        public static RelationshipMetadataModel ToModel(this OneToManyRelationshipMetadata relationship, Models.RelationshipType type)
        {
            return new RelationshipMetadataModel
            {
                SchemaName = relationship.SchemaName,
                Type = type,
                ReferencedEntity = relationship.ReferencedEntity,
                ReferencingEntity = relationship.ReferencingEntity,
                ReferencingAttribute = relationship.ReferencingAttribute
            };
        }

        public static RelationshipMetadataModel ToModel(this ManyToManyRelationshipMetadata relationship, string currentEntity)
        {
            var otherEntity = relationship.Entity1LogicalName == currentEntity
                ? relationship.Entity2LogicalName
                : relationship.Entity1LogicalName;

            return new RelationshipMetadataModel
            {
                SchemaName = relationship.SchemaName,
                Type = Models.RelationshipType.ManyToMany,
                ReferencedEntity = otherEntity,
                ReferencingEntity = currentEntity,
                ReferencingAttribute = relationship.IntersectEntityName
            };
        }
    }
}
