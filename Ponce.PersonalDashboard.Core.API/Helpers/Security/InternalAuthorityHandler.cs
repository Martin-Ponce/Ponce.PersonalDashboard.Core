namespace Ponce.PersonalDashboard.Core.API.Helpers.Security
{
    /// <summary>Sends the API's calls to the identity provider (discovery, signing keys) to the internal address instead of the public one. Every call is rewritten since the discovery doc still lists public URLs.</summary>
    public class InternalAuthorityHandler(Uri publicAuthority, Uri internalAuthority) : DelegatingHandler(new HttpClientHandler())
    {
        private readonly Uri _publicAuthority = publicAuthority;
        private readonly Uri _internalAuthority = internalAuthority;

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request.RequestUri is { } uri &&
                Uri.Compare(uri, _publicAuthority, UriComponents.SchemeAndServer, UriFormat.Unescaped, StringComparison.OrdinalIgnoreCase) == 0)
            {
                request.RequestUri = new UriBuilder(uri)
                {
                    Scheme = _internalAuthority.Scheme,
                    Host = _internalAuthority.Host,
                    Port = _internalAuthority.Port
                }.Uri;
            }
            return base.SendAsync(request, cancellationToken);
        }
    }
}
