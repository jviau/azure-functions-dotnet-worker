using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Google.Protobuf.Collections;
using Microsoft.Azure.Functions.Worker.Core.FunctionMetadata;
using Microsoft.Azure.Functions.Worker.Grpc.Messages;

namespace Microsoft.Azure.Functions.Worker.Grpc.FunctionMetadata
{
    public class HttpBindingInfo
    {
        [JsonPropertyName("name")]
        public string? Name { set; get; }

        [JsonPropertyName("type")]

        public string? Type { set; get; }

        [JsonPropertyName("direction")]

        public string? Direction { set; get; }

        [JsonPropertyName("authLevel")]

        public string? AuthLevel { set; get; }

        [JsonPropertyName("methods")]

        public string[] Methods { set; get; }
    }

    [JsonSourceGenerationOptions(WriteIndented = true)]
    [JsonSerializable(typeof(HttpBindingInfo))]
    public partial class SourceGenerationContext : JsonSerializerContext
    {
    }

    internal static class FunctionMetadataRpcExtensions
    {
        internal static MapField<string, BindingInfo> GetBindingInfoList(this IFunctionMetadata funcMetadata)
        {
            if (funcMetadata is RpcFunctionMetadata rpcFuncMetadata)
            {
                return rpcFuncMetadata.Bindings;
            }

            MapField<string, BindingInfo> bindings = new MapField<string, BindingInfo>();
            var rawBindings = funcMetadata.RawBindings;

            if (rawBindings.Count == 0)
            {
                throw new FormatException("At least one binding must be declared in a Function.");
            }
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            foreach (var bindingJson in rawBindings)
            {
       //         var b = new HttpBindingInfo { Name = "Foo", Type = "Bar" };

       //         var jsonString = JsonSerializer.Serialize(
       // b, SourceGenerationContext.Default.HttpBindingInfo);

       //         var binding23 = JsonSerializer.Deserialize<HttpBindingInfo>(
       //jsonString, SourceGenerationContext.Default.HttpBindingInfo);

                //var binding = JsonSerializer.Deserialize<JsonElement>(bindingJson);
                //var binding = JsonSerializer.Deserialize<HttpBindingInfo>(bindingJson);
                var binding = JsonSerializer.Deserialize<HttpBindingInfo>(
    bindingJson, SourceGenerationContext.Default.HttpBindingInfo);
                var binding2 = JsonSerializer.Deserialize(
    bindingJson, typeof(HttpBindingInfo), SourceGenerationContext.Default)
    as HttpBindingInfo;

                BindingInfo bindingInfo = CreateBindingInfoNew(binding);
                //binding.TryGetProperty("name", out JsonElement jsonName);
                // bindings.Add(jsonName.ToString()!, bindingInfo);
                bindings.Add(binding.Name, bindingInfo);
            }

            return bindings;
        }

        internal static BindingInfo CreateBindingInfoNew(HttpBindingInfo binding)
        {
            BindingInfo bindingInfo = new BindingInfo
            {
                Direction = string.Equals(binding.Direction, "In", StringComparison.OrdinalIgnoreCase)
                    ? BindingInfo.Types.Direction.In : BindingInfo.Types.Direction.Out,
                Type = binding.Type
            };

            return bindingInfo;
        }
        internal static BindingInfo CreateBindingInfo(JsonElement binding)
        {
            var hasDirection = binding.TryGetProperty("direction", out JsonElement jsonDirection);
            var hasType = binding.TryGetProperty("type", out JsonElement jsonType);

            if (!hasDirection
                || !hasType
                || !Enum.TryParse(jsonDirection.ToString()!, out BindingInfo.Types.Direction direction))
            {
                throw new FormatException("Bindings must declare a direction and type.");
            }

            BindingInfo bindingInfo = new BindingInfo
            {
                Direction = direction,
                Type = jsonType.ToString()
            };

            var hasDataType = binding.TryGetProperty("dataType", out JsonElement jsonDataType);

            if (hasDataType)
            {
                if (!Enum.TryParse(jsonDataType.ToString()!, out BindingInfo.Types.DataType dataType))
                {
                    throw new FormatException("Invalid DataType for a binding.");
                }

                bindingInfo.DataType = dataType;
            }

            return bindingInfo;
        }
    }
}
