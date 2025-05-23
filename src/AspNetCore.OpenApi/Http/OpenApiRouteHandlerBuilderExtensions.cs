// -----------------------------------------------------------------------
// <copyright file="OpenApiRouteHandlerBuilderExtensions.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.AspNetCore.Http;

using Microsoft.AspNetCore.Builder;

/// <summary>
/// The <see cref="OpenApi"/> <see cref="RouteHandlerBuilder"/> extensions.
/// </summary>
public static class OpenApiRouteHandlerBuilderExtensions
{
    /// <summary>
    /// Adds the <see cref="Metadata.IScopesMetadata"/> to <see cref="EndpointBuilder.Metadata"/> for all endpoints produced by <paramref name="builder"/>.
    /// </summary>
    /// <remarks>
    /// The OpenAPI specification supports scopes in the security scheme for an endpoint.
    /// </remarks>
    /// <typeparam name="TBuilder">The endpoint convention builder.</typeparam>
    /// <param name="builder">The <see cref="IEndpointConventionBuilder"/>.</param>
    /// <param name="scopes">A collection of scopes to be associated with the endpoint.</param>
    /// <returns>A <see cref="IEndpointConventionBuilder"/> that can be used to further customize the endpoint.</returns>
    public static TBuilder WithScopes<TBuilder>(this TBuilder builder, params string[] scopes)
        where TBuilder : IEndpointConventionBuilder
        => builder.WithMetadata(new ScopesAttribute(scopes));

    /// <summary>
    /// Adds the <see cref="Metadata.IScopesMetadata"/> to <see cref="EndpointBuilder.Metadata"/> for all endpoints produced by <paramref name="builder"/>.
    /// </summary>
    /// <remarks>
    /// The OpenAPI specification supports scopes in the security scheme for an endpoint.
    /// </remarks>
    /// <param name="builder">The <see cref="RouteHandlerBuilder"/>.</param>
    /// <param name="scopes">A collection of scopes to be associated with the endpoint.</param>
    /// <returns>A <see cref="RouteHandlerBuilder"/> that can be used to further customize the endpoint.</returns>
    public static RouteHandlerBuilder WithScopes(this RouteHandlerBuilder builder, params string[] scopes)
        => WithScopes<RouteHandlerBuilder>(builder, scopes);
}