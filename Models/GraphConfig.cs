
namespace Ordo.Models
{
    internal class GraphConfig
    {
        public string? ClientId { get; set; }
        public string? TenantId { get; set; }
        public string? ClientSecret { get; set; }
        public string? UserId { get; set; }

        public bool Validate(out string errorMessage)
        {
            var missingFields = new List<string>();

            if (string.IsNullOrEmpty(ClientId)) missingFields.Add(nameof(ClientId));
            if (string.IsNullOrEmpty(TenantId)) missingFields.Add(nameof(TenantId));
            if (string.IsNullOrEmpty(ClientSecret)) missingFields.Add(nameof(ClientSecret));
            if (string.IsNullOrEmpty(UserId)) missingFields.Add(nameof(UserId));

            if (missingFields.Any()) {
                errorMessage = $"The following configuration fields are missing or empty: {string.Join(", ", missingFields)}";
                return false;
            }

            errorMessage = string.Empty;
            return true;
        }
    }
}
