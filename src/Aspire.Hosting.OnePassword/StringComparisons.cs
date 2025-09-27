using System;


namespace Aspire.Hosting.OnePassword;

internal static class StringComparisons
{
    public static StringComparison ResourceName => StringComparison.OrdinalIgnoreCase;
    public static StringComparison ResourceState => StringComparison.OrdinalIgnoreCase;
    public static StringComparison EndpointAnnotationName => StringComparison.OrdinalIgnoreCase;
    public static StringComparison EndpointAnnotationUriScheme => StringComparison.OrdinalIgnoreCase;
    public static StringComparison ResourceType => StringComparison.Ordinal;
    public static StringComparison ResourcePropertyName => StringComparison.Ordinal;
    public static StringComparison ResourceOwnerName => StringComparison.Ordinal;
    public static StringComparison ResourceOwnerKind => StringComparison.Ordinal;
    public static StringComparison ResourceOwnerUid => StringComparison.Ordinal;
    public static StringComparison UserTextSearch => StringComparison.CurrentCultureIgnoreCase;
    public static StringComparison EnvironmentVariableName => StringComparison.InvariantCultureIgnoreCase;
    public static StringComparison Url => StringComparison.OrdinalIgnoreCase;
    public static StringComparison UrlPath => StringComparison.OrdinalIgnoreCase;
    public static StringComparison UrlHost => StringComparison.OrdinalIgnoreCase;
    public static StringComparison HtmlAttribute => StringComparison.Ordinal;
    public static StringComparison GridColumn => StringComparison.Ordinal;
    public static StringComparison OtlpAttribute => StringComparison.Ordinal;
    public static StringComparison OtlpFieldValue => StringComparison.OrdinalIgnoreCase;
    public static StringComparison OtlpSpanId => StringComparison.Ordinal;
    public static StringComparison HealthReportPropertyValue => StringComparison.Ordinal;
    public static StringComparison ConsoleLogContent => StringComparison.Ordinal;
    public static StringComparison CultureName => StringComparison.OrdinalIgnoreCase;
    public static StringComparison CommandName => StringComparison.Ordinal;
    public static StringComparison CliInputOrOutput => StringComparison.Ordinal;
}
