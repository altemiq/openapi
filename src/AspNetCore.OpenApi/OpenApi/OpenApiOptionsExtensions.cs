// -----------------------------------------------------------------------
// <copyright file="OpenApiOptionsExtensions.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.AspNetCore.OpenApi;

using Microsoft.OpenApi;
using Microsoft.OpenApi.Models;

/// <summary>
/// The <see cref="OpenApiOptions"/> extensions.
/// </summary>
public static class OpenApiOptionsExtensions
{
    private const string UnauthorizedValue = "User is not authorised";
    private const string ForbiddenValue = "User access to resource is forbidden";

    private static readonly string UnauthorizedKey = Http.StatusCodes.Status401Unauthorized.ToString(System.Globalization.CultureInfo.InvariantCulture);
    private static readonly string ForbiddenKey = Http.StatusCodes.Status403Forbidden.ToString(System.Globalization.CultureInfo.InvariantCulture);

    /// <summary>
    /// Sets the information on the document.
    /// </summary>
    /// <param name="options">The options.</param>
    /// <param name="title">The title.</param>
    /// <returns>The input options.</returns>
    public static OpenApiOptions SetInfo(this OpenApiOptions options, string title) => options.SetInfo(title, options.DocumentName);

    /// <summary>
    /// Sets the information on the document.
    /// </summary>
    /// <param name="options">The options.</param>
    /// <param name="title">The title.</param>
    /// <param name="version">The open-api version.</param>
    /// <returns>The input options.</returns>
    public static OpenApiOptions SetInfo(this OpenApiOptions options, string title, string version) => options.AddDocumentTransformer(
        (document, _, _) =>
        {
            document.Info.Title = $"{title} | {version}";
            document.Info.Version = GetVersion();
            return Task.CompletedTask;

            static string GetVersion()
            {
                return GetVersionFromEntryAssembly() ?? "1.0.0";

                static string? GetVersionFromEntryAssembly()
                {
                    return System.Reflection.Assembly.GetEntryAssembly()?.GetCustomAttributes(inherit: false) is { Length: > 0 } attributes
                        ? GetVersionFromAttributes(attributes)
                        : default;

                    static string? GetVersionFromAttributes(object[] attributes)
                    {
                        return GetAttributeValue<System.Reflection.AssemblyVersionAttribute>(attributes, a => a.Version)
                            ?? GetAttributeValue<System.Reflection.AssemblyFileVersionAttribute>(attributes, a => a.Version)
                            ?? GetAttributeValue<System.Reflection.AssemblyInformationalVersionAttribute>(attributes, a => a.InformationalVersion);

                        static string? GetAttributeValue<T>(object[] attributes, Func<T, string> func)
                        {
                            return attributes.OfType<T>().FirstOrDefault() is { } attribute ? func(attribute) : default;
                        }
                    }
                }
            }
        });

    /// <summary>
    /// Sets the server to have the correct path base.
    /// </summary>
    /// <param name="options">The options.</param>
    /// <param name="pathBase">The path base.</param>
    /// <returns>The input options.</returns>
    public static OpenApiOptions UsePathBase(this OpenApiOptions options, Http.PathString pathBase) => options.AddDocumentTransformer(
        (document, _, _) =>
        {
            if (document.Servers is null)
            {
                document.Servers = [new() { Url = pathBase }];
            }
            else
            {
                document.Servers.Clear();
                document.Servers.Add(new() { Url = pathBase });
            }

            return Task.CompletedTask;
        });

    /// <summary>
    /// Adds a security scheme for an API key.
    /// </summary>
    /// <param name="options">The options.</param>
    /// <returns>The scheme.</returns>
    public static (OpenApiSecurityScheme Scheme, string Name) AddApiKey(this OpenApiOptions options) => options.AddSecurityScheme(SecuritySchemeType.ApiKey);

    /// <summary>
    /// Adds a security scheme for an API key.
    /// </summary>
    /// <param name="options">The options.</param>
    /// <param name="name">The name of the reference.</param>
    /// <returns>The scheme.</returns>
    public static OpenApiSecurityScheme AddApiKey(this OpenApiOptions options, string name) => options.AddSecurityScheme(SecuritySchemeType.ApiKey, name);

    /// <summary>
    /// Adds a security scheme for HTTP.
    /// </summary>
    /// <param name="options">The options.</param>
    /// <returns>The scheme.</returns>
    public static (OpenApiSecurityScheme Scheme, string Name) AddHttp(this OpenApiOptions options) => options.AddSecurityScheme(SecuritySchemeType.Http, scheme => scheme.Scheme = "bearer");

    /// <summary>
    /// Adds a security scheme for HTTP.
    /// </summary>
    /// <param name="options">The options.</param>
    /// <param name="name">The name of the reference.</param>
    /// <returns>The scheme.</returns>
    public static OpenApiSecurityScheme AddHttp(this OpenApiOptions options, string name) => options.AddSecurityScheme(SecuritySchemeType.Http, name, scheme => scheme.Scheme = "bearer");

    /// <summary>
    /// Adds a security scheme for OAuth2.
    /// </summary>
    /// <param name="options">The options.</param>
    /// <param name="flows">An object containing configuration information for the flow types supported.</param>
    /// <returns>The scheme.</returns>
    public static (OpenApiSecurityScheme Scheme, string Name) AddOAuth2(this OpenApiOptions options, OpenApiOAuthFlows flows) => options.AddSecurityScheme(SecuritySchemeType.OAuth2, scheme => scheme.Flows = flows);

    /// <summary>
    /// Adds a security scheme for OAuth2.
    /// </summary>
    /// <param name="options">The options.</param>
    /// <param name="name">The name of the reference.</param>
    /// <param name="flows">An object containing configuration information for the flow types supported.</param>
    /// <returns>The scheme.</returns>
    public static OpenApiSecurityScheme AddOAuth2(this OpenApiOptions options, string name, OpenApiOAuthFlows flows) => options.AddSecurityScheme(SecuritySchemeType.OAuth2, name, scheme => scheme.Flows = flows);

    /// <summary>
    /// Adds a security scheme for OpenID Connect.
    /// </summary>
    /// <param name="options">The options.</param>
    /// <param name="openIdConnectUrl">The OpenID connection URL.</param>
    /// <returns>The scheme.</returns>
    public static (OpenApiSecurityScheme Scheme, string Name) AddOpenIdConnect(this OpenApiOptions options, string openIdConnectUrl) =>
        options.AddSecurityScheme(SecuritySchemeType.OpenIdConnect, scheme => scheme.OpenIdConnectUrl = new(openIdConnectUrl));

    /// <summary>
    /// Adds a security scheme for OpenID Connect.
    /// </summary>
    /// <param name="options">The options.</param>
    /// <param name="openIdConnectUrl">The OpenID connection URL.</param>
    /// <param name="name">The name of the reference.</param>
    /// <returns>The scheme.</returns>
    public static OpenApiSecurityScheme AddOpenIdConnect(this OpenApiOptions options, string openIdConnectUrl, string name) =>
        options.AddSecurityScheme(SecuritySchemeType.OpenIdConnect, name, scheme => scheme.OpenIdConnectUrl = new(openIdConnectUrl));

    /// <summary>
    /// Adds the specified security scheme.
    /// </summary>
    /// <param name="options">The options.</param>
    /// <param name="securityScheme">The security scheme.</param>
    /// <param name="name">The name of the scheme that was added.</param>
    /// <returns>The input options.</returns>
    public static OpenApiOptions AddSecurityScheme(this OpenApiOptions options, OpenApiSecurityScheme securityScheme, out string name)
    {
        name = GetName(securityScheme);
        return options.AddSecurityScheme(name, securityScheme);

        static string GetName(OpenApiSecurityScheme securityScheme)
        {
            return securityScheme switch
            {
                { Name: { } name } => name,
                { Type: { } type } => type.ToString(),
                { Scheme: { } scheme } => scheme,
                _ => securityScheme.ToString()!,
            };
        }
    }

    /// <summary>
    /// Adds the specified security scheme.
    /// </summary>
    /// <param name="options">The options.</param>
    /// <param name="name">The scheme name.</param>
    /// <param name="securityScheme">The security scheme.</param>
    /// <returns>The input options.</returns>
    public static OpenApiOptions AddSecurityScheme(this OpenApiOptions options, string name, OpenApiSecurityScheme securityScheme) => options.AddDocumentTransformer(
        (document, _, _) =>
        {
            document.Components ??= new();
            if (document.Components.SecuritySchemes is null)
            {
                document.Components.SecuritySchemes = new Dictionary<string, IOpenApiSecurityScheme>(StringComparer.OrdinalIgnoreCase)
                {
                    {
                        name,
                        securityScheme
                    },
                };
            }
            else
            {
                document.Components.SecuritySchemes.Add(name, securityScheme);
            }

            return Task.CompletedTask;
        });

    /// <summary>
    /// Adds the claims binding check to the operations.
    /// </summary>
    /// <param name="options">The options.</param>
    /// <returns>The input options.</returns>
    public static OpenApiOptions WithClaimsBindingCheck(this OpenApiOptions options) => options
        .AddOperationTransformer((operation, context, cancellationToken) =>
        {
            if (context.Description.ParameterDescriptions
                .Where(a => a.Source.DisplayName is "ClaimsBindingSource")
                .Select(i => i.Name)
                .ToArray() is { Length: > 0 } parameterNamesToRemove)
            {
                RemoveAll(operation.Parameters, [.. SafeIntersectBy(operation.Parameters, parameterNamesToRemove, p => p.Name)], cancellationToken);

                static void RemoveAll<T>(IList<T>? values, IEnumerable<T> toRemove, CancellationToken cancellationToken)
                {
                    if (values is null)
                    {
                        return;
                    }

                    foreach (var remove in toRemove)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        _ = values.Remove(remove);
                    }
                }

                static IEnumerable<TSource> SafeIntersectBy<TSource, TKey>(IEnumerable<TSource>? first, IEnumerable<TKey> second, Func<TSource, TKey> keySelector)
                {
                    return first is null ? [] : first.IntersectBy(second, keySelector);
                }
            }

            return Task.CompletedTask;
        });

    /// <summary>
    /// Adds a check for <see cref="Authorization.IAuthorizeData"/> and adds the specified scheme.
    /// </summary>
    /// <param name="options">The options.</param>
    /// <param name="name">The authentication scheme.</param>
    /// <returns>The input options.</returns>
    public static OpenApiOptions WithAuthorizeCheck(this OpenApiOptions options, string name) =>
        options.WithAuthorizeCheck((_, document) => new(name, document));

    /// <summary>
    /// Adds a check for <see cref="Authorization.IAuthorizeData"/> and adds the specified scheme.
    /// </summary>
    /// <param name="options">The options.</param>
    /// <param name="securityScheme">The authentication scheme.</param>
    /// <returns>The input options.</returns>
    public static OpenApiOptions WithAuthorizeCheck(this OpenApiOptions options, OpenApiSecuritySchemeReference? securityScheme) =>
        options.WithAuthorizeCheck((_, _) => securityScheme);

    private static OpenApiOptions WithAuthorizeCheck(this OpenApiOptions options, Func<OpenApiOptions, OpenApiDocument?, OpenApiSecuritySchemeReference?> securitySchemeFactory) => options.AddOperationTransformer(
        (operation, context, cancellationToken) =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (context.Description.ActionDescriptor.EndpointMetadata.Any(m => m is Authorization.IAuthorizeData))
            {
                operation.Responses = [];
                _ = operation.Responses.AddOrUpdate(UnauthorizedKey, _ => new OpenApiResponse { Description = UnauthorizedValue }, (_, r) =>
                {
                    r.Description = UnauthorizedValue;
                    return r;
                });
                _ = operation.Responses.AddOrUpdate(ForbiddenKey, _ => new OpenApiResponse { Description = ForbiddenValue }, (_, r) =>
                {
                    r.Description = ForbiddenValue;
                    return r;
                });

                if (securitySchemeFactory(options, context.Document) is { } securityScheme)
                {
                    var requirements = context.Description.ActionDescriptor.EndpointMetadata
                        .OfType<Http.Metadata.IScopesMetadata>()
                        .Aggregate(
                            new List<string>(),
                            (scopes, metadata) =>
                            {
                                scopes.AddRange(metadata.Scopes);
                                return scopes;
                            });

                    operation.Security ??= [];
                    operation.Security.Add(new() { [securityScheme] = requirements });
                }
            }

            return Task.CompletedTask;
        });

    private static (OpenApiSecurityScheme Scheme, string Name) AddSecurityScheme(this OpenApiOptions options, SecuritySchemeType type, Action<OpenApiSecurityScheme>? configure = default)
    {
        var securityScheme = GetSecurityScheme(type, configure);
        _ = options.AddSecurityScheme(securityScheme, out var name);
        return (securityScheme, name);
    }

    private static OpenApiSecurityScheme AddSecurityScheme(this OpenApiOptions options, SecuritySchemeType type, string name, Action<OpenApiSecurityScheme>? configure = default)
    {
        var securityScheme = GetSecurityScheme(type, configure);
        _ = options.AddSecurityScheme(name, securityScheme);
        return securityScheme;
    }

    private static OpenApiSecurityScheme GetSecurityScheme(SecuritySchemeType type, Action<OpenApiSecurityScheme>? configure = default)
    {
        var securityScheme = new OpenApiSecurityScheme
        {
            Type = type,
        };

        configure?.Invoke(securityScheme);
        return securityScheme;
    }
}