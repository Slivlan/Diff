using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    [Table("Diffs")]
    public class Diff
    {
        [Key]
        public int Id { get; set; }
        public Byte[]? Left { get; set; }
        public Byte[]? Right { get; set; }

        public Diff(int id) 
        {
            this.Id = id;
        }
    }
}
