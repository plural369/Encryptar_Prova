using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Prova_Encripitar.Models
{
    public class EncriptarModel
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Informe uma mensagem.")]
        public String Mensagem { get; set; }
    }
}