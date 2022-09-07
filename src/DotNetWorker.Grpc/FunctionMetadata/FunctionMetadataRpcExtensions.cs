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
        public string? name { set; get; }

        [JsonPropertyName("type")]

        public string? type { set; get; }

        [JsonPropertyName("direction")]

        public string? direction { set; get; }

        [JsonPropertyName("authLevel")]

        public string? authLevel { set; get; }

        [JsonPropertyName("methods")]

        public string[] methods { set; get; }
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
       
                var binding = JsonSerializer.Deserialize<HttpBindingInfo>(
    bindingJson, SourceGenerationContext.Default.HttpBindingInfo);

                if (string.IsNullOrEmpty(binding.type))
                {
                    throw new Exception($"Failed to Deserialize. bindingJson: {bindingJson}. funcMetadata.Name:{funcMetadata.Name}");
                }

                BindingInfo bindingInfo = CreateBindingInfoNew(binding);
                //binding.TryGetProperty("name", out JsonElement jsonName);
                // bindings.Add(jsonName.ToString()!, bindingInfo);
                bindings.Add(binding.name, bindingInfo);
            }

            return bindings;
        }

        internal static BindingInfo CreateBindingInfoNew(HttpBindingInfo binding)
        {
            try
            {
                BindingInfo bindingInfo = new BindingInfo
                {
                    Direction = string.Equals(binding.direction, "In", StringComparison.OrdinalIgnoreCase)
                        ? BindingInfo.Types.Direction.In : BindingInfo.Types.Direction.Out,
                    Type = binding.type
                };

                return bindingInfo;
            }
            catch(Exception ex)
            {
                var st = $"Type:{binding.type}, Direction:{binding.direction}, AuthLevel: {binding.authLevel}";
                throw new Exception($"Error in CreateBindingInfoNew.binding:{st}", ex);
            }


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
