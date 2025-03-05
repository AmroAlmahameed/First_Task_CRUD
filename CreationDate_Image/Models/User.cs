using System.ComponentModel.DataAnnotations;

namespace CreationDate_Image.Models
{
    public class User
    {
        public int Id { get; set; }

        public string UserName { get; set; }

        public DateTime CreationDate { get; set; }

        public string Image { get; set; }
    }
}
