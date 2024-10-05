using System.ComponentModel.DataAnnotations;

namespace API.Dtos
{
    public class ClientDtos
    {
        public int Id { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string EmailAddress { get; set; }

        [Required]
        public string PhoneNumber { get; set; }
    }
}
