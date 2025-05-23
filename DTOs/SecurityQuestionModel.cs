using System.ComponentModel.DataAnnotations;

namespace Inventory_Managment_System.DTOs
{
    public class SecurityQuestionModel
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Answer is required.")]
        public string Answer { get; set; }
    }
}