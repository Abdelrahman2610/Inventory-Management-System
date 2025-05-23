// File: Models/Inventory.cs
using System.ComponentModel.DataAnnotations.Schema;


namespace Inventory_Managment_System.Models
{
	public class GlassFixationCategory{
		public int id { get; set; }
		public string CatName { get; set; }
		public decimal fixation_cost { get; set; }

		public ICollection<Product> Products { get; set; } // Add this line
	}


}
