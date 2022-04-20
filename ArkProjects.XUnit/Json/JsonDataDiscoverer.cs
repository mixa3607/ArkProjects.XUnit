using Xunit.Abstractions;
using Xunit.Sdk;

namespace ArkProjects.XUnit.Json
{
    /// <summary>
    /// Implementation of <see cref="IDataDiscoverer"/> used to discover the data
    /// provided by <see cref="JsonDataAttribute"/>.
    /// </summary>
    public class JsonDataDiscoverer : DataDiscoverer
    {
        /// <inheritdoc/>
        public override bool SupportsDiscoveryEnumeration(IAttributeInfo dataAttribute, IMethodInfo testMethod)
        {
            return !dataAttribute.GetNamedArgument<bool>(nameof(JsonDataAttribute.DisableDiscoveryEnumeration));
        }
    }
}