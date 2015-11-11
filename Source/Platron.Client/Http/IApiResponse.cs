namespace Platron.Client.Http
{
    /// <summary>
    ///     A response from an API call that includes the deserialized object instance.
    /// </summary>
    public interface IApiResponse<out TPlainResponse>
    {
        /// <summary>
        ///     Object deserialized from the JSON response body.
        /// </summary>
        TPlainResponse Body { get; }

        /// <summary>
        ///     The original non-deserialized http response.
        /// </summary>
        IHttpResponse HttpResponse { get; }
    }
}