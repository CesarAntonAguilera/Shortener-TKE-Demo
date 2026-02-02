using Shortener_TKE_Demo.Contracts;
using Shortener_TKE_Demo.Repositories;
using System.Security.Cryptography;

namespace Shortener_TKE_Demo.Services
{
    public class UrlService : IUrlService
    {
        private readonly IUrlRepository _repo;

        private const int CodeLength = 7;

        // Base62 charset
        private static readonly char[] Base62 =
            "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();

        public UrlService(IUrlRepository repo)
        {
            _repo = repo;
        }

        public ShortenUrlResponseDto Shorten(string longUrl, string baseUrl)
        {
            if (!IsValidHttpUrl(longUrl))
                throw new ArgumentException("Invalid URL. Only http/https are allowed.", nameof(longUrl));

            // Idempotente: si ya existe, devolvemos el mismo código
            if (_repo.GetCodeByLongUrl(longUrl, out var existingCode))
            {
                return new ShortenUrlResponseDto(
                    LongUrl: longUrl,
                    ShortCode: existingCode,
                    ShortUrl: Combine(baseUrl, existingCode)
                );
            }

            // Generamos código evitando colisión
            string code;
            do
            {
                code = GenerateCode(CodeLength);
            }
            while (_repo.GetLongUrlByCode(code, out _));

            _repo.SaveMapping(longUrl, code);

            return new ShortenUrlResponseDto(
                LongUrl: longUrl,
                ShortCode: code,
                ShortUrl: Combine(baseUrl, code)
            );
        }

        public bool Resolve(string shortCode, out string longUrl)
            => _repo.GetLongUrlByCode(shortCode, out longUrl);

        private static bool IsValidHttpUrl(string url)
        {
            if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
                return false;

            return uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps;
        }

        private static string Combine(string baseUrl, string code)
        {
            // baseUrl como "https://localhost:5001" → "https://localhost:5001/{code}"
            return baseUrl.TrimEnd('/') + "/" + code;
        }

        private static string GenerateCode(int length)
        {
            // Random criptográfico (evita predictibilidad simple)
            Span<byte> bytes = stackalloc byte[length];
            RandomNumberGenerator.Fill(bytes);

            Span<char> chars = stackalloc char[length];
            for (int i = 0; i < length; i++)
            {
                chars[i] = Base62[bytes[i] % Base62.Length];
            }

            return new string(chars);
        }
    }
}
