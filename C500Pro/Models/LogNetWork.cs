using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace C500Pro.Models
{
    [Table("LogNetWorks")]
    public class LogNetWork
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [StringLength(10)]
        public string Mode { get; set; }

        [StringLength(2048)]
        public string Url { get; set; }
    }
}
