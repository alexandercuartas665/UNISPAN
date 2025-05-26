using BlazorInputFile;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace adesoft.adepos.webview.Data.Model
{
    public class Parameter : BaseEntity
    {

        public Parameter()
        {

        }
        [Key]
        public long ParameterId { get; set; }

        public string Description { get; set; }

        public string Module { get; set; }

        public string ValueType { get; set; }

        public string Value { get; set; }

        public string Value2 { get; set; }

        public string NameIdentify { get; set; }

        [NotMapped]
        public string AuxValue { get; set; }

        [NotMapped]
        public string Fullpath { get; set; }

        [NotMapped]
        public string NameFile { get; set; }
        [NotMapped]
        [JsonIgnore]
        public IFileListEntry FileEntry { get; set; }

        [NotMapped]
        [JsonIgnore]
        public byte[] FileBuffer { get; set; }

    }
}
