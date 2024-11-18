// -----------------------------------------------------------------------
// <copyright file="IScopesMetadata.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.AspNetCore.Http.Metadata;

/// <summary>
/// Defines a contract used to specify a collection of scopes in <see cref="Endpoint.Metadata"/>.
/// </summary>
public interface IScopesMetadata
{
    /// <summary>
    /// Gets the collection of scopes associated with the endpoint.
    /// </summary>
    IReadOnlyList<string> Scopes { get; }
}