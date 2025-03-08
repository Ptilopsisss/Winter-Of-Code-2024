using SastImg.Client.Service.API;

namespace SastImg.Client.Model
{
    public record AlbumModel
    {
        public long Id { get; set; }
        public string Title { get; set; } = "";
        public long Author { get; set; }
        public long Category { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public IDictionary<string, object> AdditionalProperties { get; set; } = new Dictionary<string, object>();
        public DateTimeOffset RemovedAt { get; private set; }
        public int AccessLevel { get; private set; }

        public string Description { get; set; } = "";

        public ICollection<long> Collaborators { get; set; } = [];

        // 将 AlbumDto 转换为 AlbumModel
        public static async Task<AlbumModel> GetModelAsync(AlbumDto dto, CancellationToken ct = default)
        {
            return new AlbumModel()
            {
                Id = dto.Id,
                Title = dto.Title,
                Author = dto.Author,
                Category = dto.Category,
                UpdatedAt = dto.UpdatedAt,
            };
        }

        public static async Task<AlbumModel> GetModelAsync(RemovedAlbumDto dto, CancellationToken ct = default)
        {
            return new AlbumModel()
            {
                Id = dto.Id,
                Title = dto.Title,
                AccessLevel = dto.AccessLevel,
                Category = dto.Category,
                RemovedAt = dto.RemovedAt,
            };
        }
    }
}

