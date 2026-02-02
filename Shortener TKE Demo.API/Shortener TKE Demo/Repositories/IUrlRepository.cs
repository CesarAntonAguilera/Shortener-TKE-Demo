namespace Shortener_TKE_Demo.Repositories
{
    public interface IUrlRepository
    {
        /// <summary>
        /// Devuelve el shortCode si la URL ya existe. Si no existe, null.
        /// </summary>
        bool GetCodeByLongUrl(string longUrl, out string shortCode);

        /// <summary>
        /// Devuelve la URL original si el shortCode existe. Si no existe, null.
        /// </summary>
        bool GetLongUrlByCode(string shortCode, out string longUrl);

        /// <summary>
        /// Guarda el mapping. Debe ser seguro en concurrencia.
        /// </summary>
        void SaveMapping(string longUrl, string shortCode);
    }
}
