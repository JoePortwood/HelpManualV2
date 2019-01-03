using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HelpManual.Entities
{
    public class FormObject
    {
        public int FormObjectId { get; set; }

        [Required(ErrorMessage = "Please use an order number")]
        public int Order { get; set; }

        public string QuestionNo { get; set; }

        public int PageNo { get; set; }

        [Required(ErrorMessage = "Please select an object type")]
        public int ObjectTypeId { get; set; }

        public virtual ObjectType ObjectType { get; set; }
    }
}
