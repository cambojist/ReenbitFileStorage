namespace FileStorage.BlobFunction.Services;

public interface ISASTokenService
{
    Task<string> GetTokenAsync(string blobName);
}
