using Shortener_TKE_Demo.Contracts;

namespace Shortener_TKE_Demo.Services
{
    public interface IUrlService
    {
        /// <summary>
        /// Crea (o devuelve existente) un shortCode para la URL.
        /// </summary>
        ShortenUrlResponseDto Shorten(string longUrl, string baseUrl);

        /// <summary>
        /// Resuelve un shortCode a su URL original.
        /// </summary>
        bool Resolve(string shortCode, out string longUrl);
    }
}
