// File: Models/Inventory.cs
using System.ComponentModel.DataAnnotations.Schema;


namespace Inventory_Managment_System.Models
{
	public class Inventory
	{
		public int Id { get; set; }

		
		public int? Product_id { get; set; }
		
		public int? Location_id { get; set; }
		public string Shelf { get; set; }

		public int Quantity { get; set; }

		public Product Product { get; set; }
		public Location Location { get; set; }
	}
}
