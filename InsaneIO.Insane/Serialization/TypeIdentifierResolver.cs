using InsaneIO.Insane.Attributes;
using InsaneIO.Insane.Exceptions;
using System.Collections.Concurrent;
using System.Reflection;
using System.Text.Json.Nodes;

namespace InsaneIO.Insane.Serialization;

public static class TypeIdentifierResolver
{
    public const string TypeIdentifierJsonPropertyName = "TypeIdentifier";
    private static readonly object TypeCacheLock = new();
    private static readonly ConcurrentDictionary<string, Type> TypeCache = new(StringComparer.Ordinal);
    private static int CachedAssemblyCount = -1;

    public static string GetTypeIdentifier(Type annotatedType)
    {
        return annotatedType.GetCustomAttribute<TypeIdentifierAttribute>()?.Identifier
            ?? throw new InvalidOperationException($"Type '{annotatedType.FullName}' is missing {nameof(TypeIdentifierAttribute)}.");
    }

    public static bool MatchesSerializedType(Type annotatedType, JsonNode jsonNode)
    {
        ArgumentNullException.ThrowIfNull(annotatedType);
        ArgumentNullException.ThrowIfNull(jsonNode);

        string? typeId = jsonNode[TypeIdentifierJsonPropertyName]?.GetValue<string>();
        return !string.IsNullOrWhiteSpace(typeId)
            && string.Equals(typeId, GetTypeIdentifier(annotatedType), StringComparison.Ordinal);
    }

    internal static Type ResolveSerializedType(Type contractType, JsonNode jsonNode, string json)
    {
        ArgumentNullException.ThrowIfNull(contractType);
        ArgumentNullException.ThrowIfNull(jsonNode);

        try
        {
            string typeId = jsonNode[TypeIdentifierJsonPropertyName]?.GetValue<string>()
                ?? throw new DeserializeException(contractType, json);
            if (string.IsNullOrWhiteSpace(typeId))
            {
                throw new DeserializeException(contractType, json);
            }

            Type implementationType = ResolveByTypeId(typeId);
            if (!contractType.IsAssignableFrom(implementationType))
            {
                throw new DeserializeException(contractType, json);
            }

            return implementationType;
        }
        catch (DeserializeException)
        {
            throw;
        }
        catch
        {
            throw new DeserializeException(contractType, json);
        }
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

    private static Type ResolveByTypeId(string typeId)
    {
        EnsureCache();
        if (!TypeCache.TryGetValue(typeId, out Type? implementationType))
        {
            throw new InvalidOperationException($"Type identifier '{typeId}' was not found.");
        }

        return implementationType;
    }

    private static void EnsureCache()
    {
        int assemblyCount = AppDomain.CurrentDomain.GetAssemblies().Length;
        if (CachedAssemblyCount == assemblyCount)
        {
            return;
        }

        lock (TypeCacheLock)
        {
            assemblyCount = AppDomain.CurrentDomain.GetAssemblies().Length;
            if (CachedAssemblyCount == assemblyCount)
            {
                return;
            }

            Dictionary<string, Type> discoveredTypes = new(StringComparer.Ordinal);

            foreach (Type type in AppDomain.CurrentDomain
                         .GetAssemblies()
                         .SelectMany(GetLoadableTypes)
                         .Where(type => !type.IsAbstract))
            {
                string? typeId = type.GetCustomAttribute<TypeIdentifierAttribute>()?.Identifier;
                if (string.IsNullOrWhiteSpace(typeId))
                {
                    continue;
                }

                if (!discoveredTypes.TryGetValue(typeId, out Type? existingType))
                {
                    discoveredTypes[typeId] = type;
                    continue;
                }

                if (existingType != type)
                {
                    throw new InvalidOperationException(
                        $"Duplicate type identifier '{typeId}' found for '{existingType.FullName}' and '{type.FullName}'.");
                }
            }

            TypeCache.Clear();
            foreach ((string key, Type value) in discoveredTypes)
            {
                TypeCache[key] = value;
            }

            CachedAssemblyCount = assemblyCount;
        }
    }
}
