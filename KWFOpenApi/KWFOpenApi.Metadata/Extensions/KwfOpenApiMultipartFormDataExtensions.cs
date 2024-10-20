namespace KWFOpenApi.Metadata.Extensions
{
    using System.Text;
    using KWFOpenApi.Metadata.Models;

    using Microsoft.OpenApi.Models;

    public static class KwfOpenApiMultipartFormDataExtensions
    {
        public static string GenerateFormBody(this OpenApiSchema value, KwfOpenApiMetadata metadata, int identation = 0, bool isFinal = true)
        {
            var reqStrBuilder = new StringBuilder("\n");
            var numProp = value.Properties.Count;
            var lastPropIndex = numProp - 1;
            //use reference like json handler
            for (int i = 0; i < numProp; i++)
            {
                var prop = value.Properties.ElementAt(i);
                if (prop.Key == null || prop.Value == null)
                {
                    continue;
                }
                reqStrBuilder.AppendIdentation(1);
                reqStrBuilder.Append(prop.Key);
                reqStrBuilder.Append(" = ");
                //reqStrBuilder.Append(FormatValueForType(prop.Value));
                //Check property is json, use json body generator
                //FormatValueForType(prop.Value, reqStrBuilder, 0, i == lastPropIndex); TODO
                reqStrBuilder.Append("\n");
            }
            reqStrBuilder.Append("\n");

            return reqStrBuilder.ToString();
        }
    }
}
