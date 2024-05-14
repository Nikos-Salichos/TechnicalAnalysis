using Dapper;
using System.Data;
using TechnicalAnalysis.CommonModels.Enums;

namespace TechnicalAnalysis.Infrastructure.Persistence.SqlHandlers
{
    public class ValueClassificationTypeHandler : SqlMapper.TypeHandler<ValueClassificationType>
    {
        public override ValueClassificationType Parse(object value)
        {
            return (ValueClassificationType)Enum.ToObject(typeof(ValueClassificationType), value);
        }
        public override void SetValue(IDbDataParameter parameter, ValueClassificationType value)
        {
            parameter.Value = (long)value;
        }
    }
}
