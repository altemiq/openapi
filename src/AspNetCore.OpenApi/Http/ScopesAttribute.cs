// -----------------------------------------------------------------------
// <copyright file="ScopesAttribute.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.AspNetCore.Http;

/// <summary>
/// Specifies a collection of scopes in <see cref="Endpoint.Metadata"/>.
/// </summary>
/// <remarks>
/// The OpenAPI specification supports scopes in the security scheme for an endpoint.
/// </remarks>
/// <param name="scopes">The scopes associated with the endpoint.</param>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Delegate | AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
[System.Diagnostics.DebuggerDisplay("{ToString(),nq}")]
public sealed class ScopesAttribute(params string[] scopes) : Attribute, Metadata.IScopesMetadata
{
    /// <inheritdoc/>
    public IReadOnlyList<string> Scopes { get; } = new List<string>(scopes);

    /// <inheritdoc/>
    public override string ToString() => $"{nameof(this.Scopes)}: {string.Join(',', this.Scopes)}";
}