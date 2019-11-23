using Microsoft.AspNetCore.Http;

using System.IO;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace API.Validation
{
    public class AllowedExtensionsAttribute : ValidationAttribute
    {
        private readonly ISet<string> extensions;

        public AllowedExtensionsAttribute(string[] extensions)
        {
            this.extensions = new HashSet<string>(extensions);
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is IFormFile file)
            {
                string fileExtension = Path.GetExtension(file.FileName);

                if (!extensions.Contains(fileExtension.ToLower()))
                {
                    return new ValidationResult(GetErrorMessage());
                }
            }

            return ValidationResult.Success;
        }

        protected string GetErrorMessage()
        {
            return "This file extension is not allowed!";
        }
    }
}
