using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.OpenApi.Models;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

public class SwaggerSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        
        if (context.MemberInfo != null)
        {
            var requiredAttribute = context.MemberInfo.GetCustomAttribute<RequiredAttribute>();
            if (requiredAttribute != null)
            {
                
                if (schema.Required != null && schema.Required.Contains(context.MemberInfo.Name))
                {
                    schema.Required.Remove(context.MemberInfo.Name); 
                }
            }
        }
    }
}
