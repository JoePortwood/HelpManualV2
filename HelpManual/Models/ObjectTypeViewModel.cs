using HelpManual.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HelpManual.Models
{
    public class ObjectTypeViewModel
    {
        public int ObjectTypeId { get; set; }

        [Required(ErrorMessage = "Please enter a name")]
        public string Name { get; set; }

        public string Data { get; set; }

        public IFormFile Image { get; set; }

        public byte[] ImageBytes { get; set; }

        public string Options { get; set; }

        public string StartEnd { get; set; }

        [Required(ErrorMessage = "Please select a control type")]
        public int ControlTypeId { get; set; }

        public virtual ControlType ControlType { get; set; }

        public virtual ICollection<FormObject> FormObject { get; set; }
    }

    public class ObjectTypeViewModelValidator : AbstractValidator<ObjectTypeViewModel>
    {
        public ObjectTypeViewModelValidator()
        {
            RuleFor(x => x.Image).NotNull().When(x => x.Data == null);
            RuleFor(x => x.Data).NotNull().When(x => x.Image == null);
        }
    }
}
