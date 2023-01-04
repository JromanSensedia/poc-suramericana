using System.ComponentModel;

namespace ServiceBusMessaging
{
    public class MyPayload
    {
        public int Id { get; set; }
        [Description("Name Sura")]
        public string Name { get; set; }        
        public string documentoID { get; set; }
        public bool Delete { get; set; }
        [Description("Id Message Sura")]
        public string messageId { get; set; } 
        [Description("DNI person Sura")]
        public string idDNI { get; set; }
        [Description("Description Document mesagge Sura")]
        public string Description { get; set; }
        [Description("File Base64 Sura")]
        public string FileSura { get; set; }
        [Description("File name Sura")]
        public string FileNameSura { get; set; }
        [Description("Extension File")]
        public string FileExtension { get; set; }
    }
}
