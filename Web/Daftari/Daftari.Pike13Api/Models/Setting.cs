using System.ComponentModel.DataAnnotations;

namespace Daftari.Pike13Api.Models
{
    public class Setting
    {
        [Key]
        [StringLength(30)]
        public string Key { get; set; }

        public string Value { get; set; }
    }
}