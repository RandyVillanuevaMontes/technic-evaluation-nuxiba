using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Evaluation_Nuxiba.Models
{
    [Table("ccUsers")]
    public class CcUser
    {
        [Key]
        public int User_id { get; set; }

        public string? Login { get; set; } = string.Empty;

        public string? Nombres { get; set; } = string.Empty;

        public string? ApellidoPaterno { get; set; } = string.Empty;

        public string? ApellidoMaterno { get; set; } = string.Empty;

        [Column("IDArea")]
        public int Area_id { get; set; }

        public ICollection<CcLogLogin>? Logins { get; set; }
    }
}