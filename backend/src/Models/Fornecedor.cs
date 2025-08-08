using System.ComponentModel.DataAnnotations;

namespace MyApp.Models
{
    public class Fornecedor
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Nome { get; set; }

        [Required]
        public string Cnpj { get; set; }

        [Required]
        public string TipoPessoa { get; set; } // Exemplo: "MEI", "EIRELI", etc.
    }
}
