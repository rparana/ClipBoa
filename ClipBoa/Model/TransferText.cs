using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClipBoa.Model
{
    [Table("TransferText")]
    public class TransferText
    {
        [Key]
        public int ID { get; set; }
        public string Texto { get; set; }

    }
}
