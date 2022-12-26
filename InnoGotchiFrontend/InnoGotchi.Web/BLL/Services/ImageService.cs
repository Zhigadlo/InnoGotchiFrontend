namespace InnoGotchi.Web.BLL.Services
{
    public class ImageService
    {
        public byte[]? GetBytesFromFormFile(IFormFile fileForm)
        {
            if (fileForm != null)
            {
                byte[] imageData = null;
                using (var binaryReader = new BinaryReader(fileForm.OpenReadStream()))
                {
                    imageData = binaryReader.ReadBytes((int)fileForm.Length);
                }
                return imageData;
            }
            else
                return null;
        }
    }
}
