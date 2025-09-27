using Aspire.Hosting.ApplicationModel;
using System.Collections.Immutable;


namespace Aspire.Hosting.OnePassword;

internal static class CustomResourceSnapshotExtensions
{
    internal static ImmutableArray<ResourcePropertySnapshot> SetResourceProperty(this ImmutableArray<ResourcePropertySnapshot> properties, string name, object value)
    {
        for (var i = 0; i < properties.Length; i++)
        {
            var property = properties[i];

            if (string.Equals(property.Name, name, StringComparisons.ResourcePropertyName))
            {
                if (property.Value == value)
                {
                    // Unchanged.
                    return properties;
                }

                // Set value.
                return properties.SetItem(i, property with { Value = value, IsSensitive = true });
            }
        }

        // Add property.
        return [.. properties, new ResourcePropertySnapshot(name, value) { IsSensitive = true }];
    }

}
