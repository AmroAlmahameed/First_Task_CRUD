namespace CreationDate_Image.Models
{
    public class UserViewModel
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public IFormFile ImageFile { get; set; }
    }
}
