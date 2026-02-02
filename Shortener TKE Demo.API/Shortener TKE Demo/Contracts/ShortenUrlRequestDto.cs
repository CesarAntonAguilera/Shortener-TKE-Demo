namespace Shortener_TKE_Demo.Contracts
{
    public sealed record ShortenUrlResponseDto(
        string LongUrl,
        string ShortCode,
        string ShortUrl
    );
}
