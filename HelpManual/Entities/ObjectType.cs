using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HelpManual.Entities
{
    public class ObjectType
    {
        public int ObjectTypeId { get; set; }

        [Required(ErrorMessage = "Please enter a name")]
        public string Name { get; set; }

        //[Required(ErrorMessage = "Please enter data e.g. image location or paragraph text")]
        public string Data { get; set; }

        public byte[] Image { get; set; }

        public string Options { get; set; }

        public string StartEnd { get; set; }

        [Required(ErrorMessage = "Please select a control type")]
        public int ControlTypeId { get; set; }

        public virtual ControlType ControlType { get; set; }

        public virtual ICollection<FormObject> FormObject { get; set; }
    }
}
