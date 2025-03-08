using SastImg.Client.Service.API;

namespace SastImg.Client.Model
{
    public record CategoryModel
    {
        public long Id { get; set; }
        public string Name { get; set; }

        public string Description { get; set; }

        public static async Task<CategoryModel> GetModelAsync(CategoryDto dto, CancellationToken ct = default)
        {
            return new CategoryModel()
            {
                Id = dto.Id,
                Name = dto.Name,
                Description = dto.Description
            };
        }

    }
}
