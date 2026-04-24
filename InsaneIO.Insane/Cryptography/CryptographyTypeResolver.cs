using InsaneIO.Insane.Cryptography.Attributes;
using InsaneIO.Insane.Exceptions;
using InsaneIO.Insane.Serialization;
using System.Collections.Concurrent;
using System.Reflection;
using System.Text.Json.Nodes;

namespace InsaneIO.Insane.Cryptography
{
    internal static class CryptographyTypeResolver
    {
        internal const string JsonPropertyName = "CryptographyType";
        private static readonly ConcurrentDictionary<Type, CachedTypeMap> TypeCache = new();

        internal static string GetTypeId(Type implementationType)
        {
            return implementationType.GetCustomAttribute<CryptographyTypeAttribute>()?.Identifier
                ?? throw new InvalidOperationException($"Type '{implementationType.FullName}' is missing {nameof(CryptographyTypeAttribute)}.");
        }

        internal static bool MatchesSerializedType(Type implementationType, JsonNode jsonNode)
        {
            ArgumentNullException.ThrowIfNull(implementationType);
            ArgumentNullException.ThrowIfNull(jsonNode);

            string? typeId = jsonNode[JsonPropertyName]?.GetValue<string>();
            if (!string.IsNullOrWhiteSpace(typeId))
            {
                return string.Equals(typeId, GetTypeId(implementationType), StringComparison.Ordinal);
            }

            string? assemblyName = jsonNode[nameof(IBaseSerializable.AssemblyName)]?.GetValue<string>();
            return string.Equals(assemblyName, IBaseSerializable.GetName(implementationType), StringComparison.Ordinal);
        }

        internal static Type ResolveSerializedType(Type contractType, JsonNode jsonNode, string json)
        {
            ArgumentNullException.ThrowIfNull(contractType);
            ArgumentNullException.ThrowIfNull(jsonNode);

            string? typeId = jsonNode[JsonPropertyName]?.GetValue<string>();
            if (!string.IsNullOrWhiteSpace(typeId))
            {
                if (TryResolveByTypeId(contractType, typeId, out Type? matchingType))
                {
                    return matchingType ?? throw new DeserializeException(contractType, json);
                }
            }

            string assemblyName = jsonNode[nameof(IBaseSerializable.AssemblyName)]?.GetValue<string>()
                ?? throw new DeserializeException(contractType, json);
            Type concreteType = Type.GetType(assemblyName) ?? throw new DeserializeException(contractType, json);

            if (!contractType.IsAssignableFrom(concreteType))
            {
                throw new DeserializeException(contractType, json);
            }

            return concreteType;
        }

        internal static object InvokeDeserialize(Type contractType, Type concreteType, string json)
        {
            MethodInfo? deserializeMethod = concreteType.GetMethod(
                "Deserialize",
                BindingFlags.Public | BindingFlags.Static,
                binder: null,
                types: [typeof(string)],
                modifiers: null);

            if (deserializeMethod is null || !contractType.IsAssignableFrom(deserializeMethod.ReturnType))
            {
                throw new DeserializeException(contractType, json);
            }

            return deserializeMethod.Invoke(null, [json]) ?? throw new DeserializeException(contractType, json);
        }

        private static IEnumerable<Type> GetLoadableTypes(Assembly assembly)
        {
            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException exception)
            {
                return exception.Types.Where(type => type is not null)!;
            }
        }

        private static bool TryResolveByTypeId(Type contractType, string typeId, out Type? implementationType)
        {
            CachedTypeMap cache = GetOrBuildCache(contractType);
            return cache.TypeMap.TryGetValue(typeId, out implementationType);
        }

        private static CachedTypeMap GetOrBuildCache(Type contractType)
        {
            int assemblyCount = AppDomain.CurrentDomain.GetAssemblies().Length;
            CachedTypeMap cache = TypeCache.GetOrAdd(contractType, _ => BuildCache(contractType, assemblyCount));

            if (cache.AssemblyCount == assemblyCount)
            {
                return cache;
            }

            CachedTypeMap refreshedCache = BuildCache(contractType, assemblyCount);
            TypeCache[contractType] = refreshedCache;
            return refreshedCache;
        }

        private static CachedTypeMap BuildCache(Type contractType, int assemblyCount)
        {
            Dictionary<string, Type> typeMap = new(StringComparer.Ordinal);

            foreach (Type type in AppDomain.CurrentDomain
                         .GetAssemblies()
                         .SelectMany(GetLoadableTypes)
                         .Where(type => !type.IsAbstract && contractType.IsAssignableFrom(type)))
            {
                string? typeId = type.GetCustomAttribute<CryptographyTypeAttribute>()?.Identifier;
                if (string.IsNullOrWhiteSpace(typeId))
                {
                    continue;
                }

                if (typeMap.TryGetValue(typeId, out Type? existingType) && existingType != type)
                {
                    throw new InvalidOperationException(
                        $"Duplicate cryptography type identifier '{typeId}' found for '{existingType.FullName}' and '{type.FullName}'.");
                }

                typeMap[typeId] = type;
            }

            return new CachedTypeMap(assemblyCount, typeMap);
        }

        private sealed record CachedTypeMap(int AssemblyCount, IReadOnlyDictionary<string, Type> TypeMap);
    }
}
