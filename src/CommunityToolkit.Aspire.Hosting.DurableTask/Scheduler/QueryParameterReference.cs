using Aspire.Hosting.ApplicationModel;
using System.Net;

namespace CommunityToolkit.Aspire.Hosting.DurableTask.Scheduler;

sealed class QueryParameterReference : IValueProvider, IValueWithReferences, IManifestExpressionProvider
{
    public static QueryParameterReference Create(ReferenceExpression reference) => new(reference);

    private readonly ReferenceExpression reference;

    private QueryParameterReference(ReferenceExpression reference)
    {
        this.reference = reference;
    }

    IEnumerable<object> IValueWithReferences.References => [this.reference];

    public async ValueTask<string?> GetValueAsync(CancellationToken cancellationToken = default)
    {
        var value = await this.reference.GetValueAsync(cancellationToken);

        return WebUtility.UrlEncode(value);
    }

    string IManifestExpressionProvider.ValueExpression => WebUtility.UrlEncode(reference.ValueExpression);
}
