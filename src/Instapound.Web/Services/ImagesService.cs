namespace Instapound.Web.Services;

public class ImagesService
{
    public async Task<string?> SaveFormImage(IFormFile? formFile, string folder)
    {
        string? fileName = null;

        if (formFile is not null && formFile.Length > 0)
        {
            var extension = Path.GetExtension(formFile.FileName);
            fileName = $"{Guid.NewGuid()}.{extension}";
            var filePath = $"{folder}{fileName}";

            using var stream = File.Create(filePath);

            await formFile.CopyToAsync(stream);
        }

        return fileName;
    }
}