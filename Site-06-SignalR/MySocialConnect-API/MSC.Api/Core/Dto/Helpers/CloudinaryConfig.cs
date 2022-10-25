namespace MSC.Api.Core.Dto.Helpers;
public class CloudinaryConfig
{
    public CloudinaryConfig() { }

    public CloudinaryConfig(string cloudName, string apiKey, string apiSecret)
    {
        CloudName = cloudName;
        ApiKey = apiKey;
        ApiSecret = apiSecret;
    }

    public string CloudName { get; set; }
    public string ApiKey { get; set; }
    public string ApiSecret { get; set; }

    //consolidated property
    public string ApiEnvironmentVariable => $"CLOUDINARY_URL=cloudinary://{ApiKey}:{ApiSecret}@{CloudName}";
}
