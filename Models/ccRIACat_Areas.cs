using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Evaluation_Nuxiba.Models
{
    [Table("ccRIACat_Areas")]
    public class ccRIACat_Areas
    {
        [Key]
        public int IDArea { get; set; }

        public string AreaName { get; set; }

        public int StatusArea { get; set; }

        public DateTime? fecha { get; set; }
    }
}
