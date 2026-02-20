namespace Evaluation_Nuxiba.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("ccloglogin")]
    public class CcLogLogin
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LoginId { get; set; }

        [Required]
        [ForeignKey("CcUser")]
        public int User_id { get; set; }

        [Required]
        public int Extension { get; set; }

        [Required]
        public int TipoMov { get; set; }

        [Required]
        public DateTime? fecha { get; set; }
        public CcUser? CcUser { get; set; }
    }
}