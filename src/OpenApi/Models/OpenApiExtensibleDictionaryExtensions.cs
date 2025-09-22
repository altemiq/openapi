// -----------------------------------------------------------------------
// <copyright file="OpenApiExtensibleDictionaryExtensions.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.OpenApi.Models;

/// <summary>
/// Extensions for <see cref="OpenApiResponses"/>.
/// </summary>
public static class OpenApiExtensibleDictionaryExtensions
{
    /// <summary>
    /// Adds or updates a response.
    /// </summary>
    /// <typeparam name="TValue">The type of value in <paramref name="dictionary"/>.</typeparam>
    /// <typeparam name="TArg">The type of factory argument.</typeparam>
    /// <param name="dictionary">The dictionary.</param>
    /// <param name="key">The key to be added or whose value should be updated.</param>
    /// <param name="addValueFactory">The function used to generate a value for an absent key.</param>
    /// <param name="updateValueFactory">The function used to generate a new value for an existing key based on the key's existing value.</param>
    /// <param name="factoryArgument">An argument to pass into <paramref name="addValueFactory"/> and <paramref name="updateValueFactory"/>.</param>
    /// <returns>The new value for the key. This will be either be the result of <paramref name="addValueFactory"/> (if the key was absent) or the result of <paramref name="updateValueFactory"/> (if the key was present).</returns>
    public static TValue AddOrUpdate<TValue, TArg>(
        this OpenApiExtensibleDictionary<TValue> dictionary,
        string key,
        Func<string, TArg, TValue> addValueFactory,
        Func<string, TValue, TArg, TValue> updateValueFactory,
        TArg factoryArgument)
        where TValue : IOpenApiSerializable
#if NET9_0_OR_GREATER
        where TArg : allows ref struct
#endif
    {
        if (dictionary.TryGetValue(key, out var value))
        {
            var updated = updateValueFactory(key, value, factoryArgument);
            dictionary[key] = updated;
            return updated;
        }

        value = addValueFactory(key, factoryArgument);
        dictionary.Add(key, value);
        return value;
    }

    /// <summary>
    /// Adds or updates a response.
    /// </summary>
    /// <typeparam name="TValue">The type of value in <paramref name="dictionary"/>.</typeparam>
    /// <param name="dictionary">The dictionary.</param>
    /// <param name="key">The key to be added or whose value should be updated.</param>
    /// <param name="addValueFactory">The function used to generate a value for an absent key.</param>
    /// <param name="updateValueFactory">The function used to generate a new value for an existing key based on the key's existing value.</param>
    /// <returns>The new value for the key. This will be either be the result of <paramref name="addValueFactory"/> (if the key was absent) or the result of <paramref name="updateValueFactory"/> (if the key was present).</returns>
    public static TValue AddOrUpdate<TValue>(
        this OpenApiExtensibleDictionary<TValue> dictionary,
        string key,
        Func<string, TValue> addValueFactory,
        Func<string, TValue, TValue> updateValueFactory)
        where TValue : IOpenApiSerializable
    {
        if (dictionary.TryGetValue(key, out var value))
        {
            var updated = updateValueFactory(key, value);
            dictionary[key] = updated;
            return updated;
        }

        value = addValueFactory(key);
        dictionary.Add(key, value);
        return value;
    }

    /// <summary>
    /// Adds or updates a response.
    /// </summary>
    /// <typeparam name="TValue">The type of value in <paramref name="dictionary"/>.</typeparam>
    /// <param name="dictionary">The dictionary.</param>
    /// <param name="key">The key to be added or whose value should be updated.</param>
    /// <param name="addValue">The value to be added for an absent key.</param>
    /// <param name="updateValueFactory">The function used to generate a new value for an existing key based on the key's existing value.</param>
    /// <returns>The new value for the key. This will be either be the value of <paramref name="addValue"/> (if the key was absent) or the result of <paramref name="updateValueFactory"/> (if the key was present).</returns>
    public static TValue AddOrUpdate<TValue>(
        this OpenApiExtensibleDictionary<TValue> dictionary,
        string key,
        TValue addValue,
        Func<string, TValue, TValue> updateValueFactory)
        where TValue : IOpenApiSerializable
    {
        if (dictionary.TryGetValue(key, out var value))
        {
            var updated = updateValueFactory(key, value);
            dictionary[key] = updated;
            return updated;
        }

        dictionary.Add(key, addValue);
        return addValue;
    }
}