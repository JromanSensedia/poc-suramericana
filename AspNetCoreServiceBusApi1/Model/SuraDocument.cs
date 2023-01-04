using System.ComponentModel.DataAnnotations;

namespace ServiceBusSenderApi.Model
{
    public class SuraDocument
    {
        [Required]
        public string  idDNI{ get; set; }
        [Required]
        public string Descripcion { get; set; }
        [Required]
        public string FileSura { get; set; }
        [Required]
        public string FileNameSura { get; set; }
        [Required]
        public string FileExtension { get; set; }
    }
}
