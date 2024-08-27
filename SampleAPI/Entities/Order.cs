using System.ComponentModel.DataAnnotations;

namespace SampleAPI.Entities
{
    public class Order
    {
        public Guid Id { get; set; }
        public DateTime EntryDate { get; set; }

        [StringLength(100, ErrorMessage = "Description cannot be longer than 100 characters.")]
        public string Description { get; set; }

        [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters.")]
        public string Name { get; set; }
        public bool Invoiced { get; set; } = true;
        public bool Deleted { get; set; } = false;

    }
}
