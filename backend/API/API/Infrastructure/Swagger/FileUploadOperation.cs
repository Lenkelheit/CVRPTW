using Microsoft.AspNetCore.Http;

using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace API.Infrastructure.Swagger
{
    public class FileUploadOperation : IOperationFilter
    {
        public void Apply(Operation operation, OperationFilterContext context)
        {
            IEnumerable<ParameterInfo> fileParams = context.MethodInfo.GetParameters()
                .Where(p => p.ParameterType.FullName.Equals(typeof(IFormFile).FullName));
            
            if (fileParams.Any() && fileParams.Count() == 1)
            {
                operation.Parameters.Clear();
                operation.Parameters.Add(new NonBodyParameter
                {
                    Name = fileParams.First().Name,
                    In = "formData",
                    Description = "Upload File",
                    Required = true,
                    Type = "file",
                });
                operation.Consumes.Add("multipart/form-data");
            }
        }
    }
}
