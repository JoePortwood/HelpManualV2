using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HelpManual.Entities
{
    public class ControlType
    {
        public int ControlTypeId { get; set; }

        [Required(ErrorMessage = "Please enter a name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please enter a control")]
        public string Control { get; set; }

        public virtual ICollection<ObjectType> ObjectType { get; set; }
    }
}
