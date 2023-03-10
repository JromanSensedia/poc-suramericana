using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ServiceBusReceiverApi.Model
{
    public class Payload
    {
        [Key]
        public int Id { get; set; }
        public string documentoID { get; set; }
        public DateTime Created { get; set; }
        [Description("Id Message Sura")]
        public string messageId { get; set; }
        [Required]
        [Description("Name Sura")]
        public string Name { get; set; }
        [Required]
        [Description("DNI person Sura")]
        public string idDNI { get; set; }
        [Description("Description Document mesagge Sura")]
        public string Description { get; set; }
        [Required]
        [Description("File Base64 Sura")]
        public string FileSura { get; set; }
        [Required]
        [Description("File name Sura")]
        public string FileNameSura { get; set; }
        [Required]
        [Description("Extension File")]
        public string FileExtension { get; set; }
    }
}
