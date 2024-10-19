namespace KWFOpenApi.Metadata.Extensions
{
    using System.Text;
    using Microsoft.OpenApi.Any;
    using Microsoft.OpenApi.Models;

    public static class KwfOpenApiJsonExtensions
    {
        private const int _maxEnumDepth = 6;
        public static string GenerateJsonBody(this OpenApiSchema value, int identation = 0, bool isFinal = true)
        {
            var reqStrBuilder = new StringBuilder();
            var numProp = value.Properties.Count;

            if (numProp > 0)
            {
                reqStrBuilder.Append("{\n");
                var lastPropIndex = numProp - 1;
                for (int i = 0; i < numProp; i++)
                {
                    var prop = value.Properties.ElementAt(i);
                    if (prop.Key == null || prop.Value == null)
                    {
                        continue;
                    }

                    reqStrBuilder.AppendIdentation(identation + 1);

                    reqStrBuilder.Append("\"");
                    reqStrBuilder.Append(prop.Key);
                    reqStrBuilder.Append("\": ");

                    reqStrBuilder.AppendPropertiesForJsonType(prop.Value, identation, i == lastPropIndex);
                }

                reqStrBuilder.AppendIdentation(identation);

                reqStrBuilder.Append("}");
                if (!isFinal)
                {
                    reqStrBuilder.Append(",");
                }
                reqStrBuilder.Append("\n");
            }

            //handle examples when no props
            if (numProp == 0)
            {
                //for dictionary
                reqStrBuilder.AppendForJsonDictionary(value, identation, isFinal);

                //handle examples for array
                reqStrBuilder.AppendForJsonArray(value, identation, isFinal);
            }

            return reqStrBuilder.ToString();
        }

        private static void AppendPropertiesForJsonType(this StringBuilder builder, OpenApiSchema value, int currIdentation, bool isFinal)
        {
            if (value?.Type == null)
            {
                return;
            }

            if (value.Type.Equals(Constants.stringType, StringComparison.InvariantCultureIgnoreCase))
            {
                var stringValue = value.Example.GetOpenApiValue().value;
                if (string.IsNullOrEmpty(stringValue))
                {
                    if (value.Enum != null && value.Enum.Count > 0)
                    {
                        var enumStrBuilder = new StringBuilder();
                        enumStrBuilder.Append(value.Type);
                        enumStrBuilder.Append(" => [");

                        var iteratorMax = value.Enum.Count > _maxEnumDepth ? _maxEnumDepth : value.Enum.Count;
                        for (int ei = 0; ei < iteratorMax; ei++)
                        {
                            var isLastEnum = (ei == iteratorMax - 1);
                            if (value.Enum[ei] is OpenApiString strEnum)
                            {
                                enumStrBuilder.Append(strEnum.Value);
                                if (!isLastEnum)
                                {
                                    enumStrBuilder.Append(","); 
                                }
                            }

                            if (isLastEnum && value.Enum.Count > iteratorMax)
                            {
                                enumStrBuilder.Append(",(...)");
                            }
                        }

                        enumStrBuilder.Append("]");
                        stringValue = enumStrBuilder.ToString();
                    }
                    else
                    {
                        stringValue = value.Format.GetStringSampleForFormat();
                    }
                }

                builder.Append("\"");
                builder.Append(stringValue);
                builder.Append("\"");
                if (!isFinal)
                {
                    builder.Append(",");
                }
                builder.Append("\n");

                return;
            }

            if (value.Type.Equals(Constants.boolType, StringComparison.InvariantCultureIgnoreCase))
            {
                var boolValue = value.Example.GetOpenApiValue().value;
                if (string.IsNullOrEmpty(boolValue))
                {
                    boolValue = bool.FalseString;
                }

                builder.Append(boolValue);
                if (!isFinal)
                {
                    builder.Append(",");
                }
                builder.Append("\n");

                return;
            }

            if (value.Type.Equals(Constants.integerType, StringComparison.InvariantCultureIgnoreCase) ||
                value.Type.Equals(Constants.numberType, StringComparison.InvariantCultureIgnoreCase))
            {
                var numberValue = value.Example.GetOpenApiValue().value;
                if (string.IsNullOrEmpty(numberValue))
                {
                    numberValue = "0";
                }

                builder.Append(numberValue);
                if (!isFinal)
                {
                    builder.Append(",");
                }
                builder.Append("\n");

                return;
            }

            if (value.Type.Equals(Constants.nullType, StringComparison.InvariantCultureIgnoreCase))
            {
                builder.Append("null");
                if (!isFinal)
                {
                    builder.Append(",");
                }
                builder.Append("\n");

                return;
            }

            if (value.Type.Equals(Constants.ObjectType, StringComparison.InvariantCultureIgnoreCase) ||
                value.Type.Equals(Constants.ArrayType, StringComparison.InvariantCultureIgnoreCase))
            {
                builder.Append(value.GenerateJsonBody(currIdentation + 1, isFinal));
                return;
            }

            return;
        }

        private static void AppendForJsonDictionary(this StringBuilder builder, OpenApiSchema value, int identation, bool isFinal)
        {
            if (value.Example == null)
            {
                if (value.Type == Constants.ObjectType)
                {
                    builder.Append("{}");
                    if (!isFinal)
                    {
                        builder.Append(",");
                    }

                    builder.Append("\n");
                }
                return;
            }

            if (value.Example.AnyType == AnyType.Object && value.Example is Dictionary<string, IOpenApiAny> exampleDict)
            {
                builder.Append("{\n");
                var dictCount = exampleDict.Count;
                for (int y = 0; y < dictCount; y++)
                {
                    var dicEntry = exampleDict.ElementAt(y);
                    var dicKey = dicEntry.Key;
                    var dicValue = dicEntry.Value.GetOpenApiValue();

                    builder.AppendIdentation(identation + 1);

                    builder.Append("\"");
                    builder.Append(dicKey);
                    builder.Append("\": ");

                    if (dicValue.type.IsStringFormat())
                    {
                        builder.Append("\"");
                        builder.Append(dicValue.value);
                        builder.Append("\"");
                    }

                    else
                    {
                        builder.Append(dicValue.value);
                    }

                    if (y < dictCount - 1)
                    {
                        builder.Append(",");
                    }

                    builder.Append("\n");
                }

                builder.AppendIdentation(identation);

                builder.Append("}");
                if (!isFinal)
                {
                    builder.Append(",");
                }

                builder.Append("\n");
            }
        }

        private static void AppendForJsonArray(this StringBuilder builder, OpenApiSchema value, int identation, bool isFinal)
        {
            if (value.Example == null)
            {
                if (value.Type == Constants.ArrayType)
                {
                    if (value.Items != null && value.Items.Type == Constants.ObjectType && value.Items.Properties != null && value.Items.Properties.Count > 0)
                    {
                        builder.Append("[\n");
                        builder.AppendIdentation(identation + 1);
                        builder.Append(value.Items.GenerateJsonBody(identation + 1));                        
                        builder.AppendIdentation(identation);
                        builder.Append("]");
                        if (!isFinal)
                        {
                            builder.Append(",");
                        }
                        builder.Append("\n");

                        return;
                    }

                    builder.Append("[]");
                    if (!isFinal)
                    {
                        builder.Append(",");
                    }

                    builder.Append("\n");
                }
                return;
            }

            if (value.Example.AnyType == AnyType.Array && value.Example is OpenApiArray exampleArray)
            {
                builder.Append("[\n");
                var count = exampleArray.Count;
                for (int y = 0; y < count; y++)
                {
                    var item = exampleArray.ElementAt(y).GetOpenApiValue();
                    if (y == 0)
                    {
                        builder.AppendIdentation(identation + 1);
                    }

                    if (item.type.IsStringFormat())
                    {
                        builder.Append("\"");
                        builder.Append(item.value);
                        builder.Append("\"");
                    }
                    else
                    {
                        builder.Append(item.value);
                    }

                    if (y < count - 1)
                    {
                        builder.Append(",");
                    }
                }

                if (count > 0)
                {
                    builder.Append("\n");
                    builder.AppendIdentation(identation);
                }

                builder.Append("]");
                if (!isFinal)
                {
                    builder.Append(",");
                }
                builder.Append("\n");
            }
        }
    }
}
