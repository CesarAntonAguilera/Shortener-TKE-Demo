using System.Collections.Concurrent;

namespace Shortener_TKE_Demo.Repositories
{
    public class UrlRepository : IUrlRepository
    {
        // code -> longUrl
        private readonly ConcurrentDictionary<string, string> _codeToLong = new(StringComparer.Ordinal);

        // longUrl -> code (para idempotencia)
        private readonly ConcurrentDictionary<string, string> _longToCode = new(StringComparer.Ordinal);

        public bool GetCodeByLongUrl(string longUrl, out string shortCode)
            => _longToCode.TryGetValue(longUrl, out shortCode!);

        public bool GetLongUrlByCode(string shortCode, out string longUrl)
            => _codeToLong.TryGetValue(shortCode, out longUrl!);

        public void SaveMapping(string longUrl, string shortCode)
        {
            // Importante: en concurrencia, dos requests podrían intentar guardar a la vez.
            // GetOrAdd hace el comportamiento idempotente de forma segura.
            var existingCode = _longToCode.GetOrAdd(longUrl, shortCode);

            // Si ya existía un código distinto para esa URL, lo respetamos.
            _codeToLong.TryAdd(existingCode, longUrl);
        }
    }
}
