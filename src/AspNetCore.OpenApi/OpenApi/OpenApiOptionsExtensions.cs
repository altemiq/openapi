// -----------------------------------------------------------------------
// <copyright file="OpenApiOptionsExtensions.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.AspNetCore.OpenApi;

using Microsoft.OpenApi.Models;

/// <summary>
/// The <see cref="OpenApiOptions"/> extensions.
/// </summary>
public static class OpenApiOptionsExtensions
{
    private const string UnauthorizedValue = "User is not authorised";
    private const string ForbiddenValue = "User access to resource is forbidden";

    private static readonly string UnauthorizedKey = AspNetCore.Http.StatusCodes.Status401Unauthorized.ToString(System.Globalization.CultureInfo.InvariantCulture);
    private static readonly string ForbiddenKey = AspNetCore.Http.StatusCodes.Status403Forbidden.ToString(System.Globalization.CultureInfo.InvariantCulture);

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
            document.Servers.Clear();
            document.Servers.Add(new OpenApiServer { Url = pathBase });
            return Task.CompletedTask;
        });

    /// <summary>
    /// Adds a security scheme for an API key.
    /// </summary>
    /// <param name="options">The options.</param>
    /// <returns>The input options.</returns>
    public static OpenApiSecurityScheme AddApiKey(this OpenApiOptions options) => options.AddApiKey(nameof(SecuritySchemeType.ApiKey));

    /// <summary>
    /// Adds a security scheme for an API key.
    /// </summary>
    /// <param name="options">The options.</param>
    /// <param name="name">The name of the reference.</param>
    /// <returns>The input options.</returns>
    public static OpenApiSecurityScheme AddApiKey(this OpenApiOptions options, string name) => options.AddApiKey(new OpenApiReference { Id = name, Type = ReferenceType.SecurityScheme });

    /// <summary>
    /// Adds a security scheme for an API key.
    /// </summary>
    /// <param name="options">The options.</param>
    /// <param name="schemeReference">The scheme reference.</param>
    /// <returns>The input options.</returns>
    public static OpenApiSecurityScheme AddApiKey(this OpenApiOptions options, OpenApiReference schemeReference) => options.AddSecurityScheme(SecuritySchemeType.ApiKey, schemeReference);

    /// <summary>
    /// Adds a security scheme for HTTP.
    /// </summary>
    /// <param name="options">The options.</param>
    /// <returns>The input options.</returns>
    public static OpenApiSecurityScheme AddHttp(this OpenApiOptions options) => options.AddHttp(nameof(SecuritySchemeType.Http));

    /// <summary>
    /// Adds a security scheme for HTTP.
    /// </summary>
    /// <param name="options">The options.</param>
    /// <param name="name">The name of the reference.</param>
    /// <returns>The input options.</returns>
    public static OpenApiSecurityScheme AddHttp(this OpenApiOptions options, string name) => options.AddHttp(new OpenApiReference { Id = name, Type = ReferenceType.SecurityScheme });

    /// <summary>
    /// Adds a security scheme for HTTP.
    /// </summary>
    /// <param name="options">The options.</param>
    /// <param name="schemeReference">The scheme reference.</param>
    /// <returns>The input options.</returns>
    public static OpenApiSecurityScheme AddHttp(this OpenApiOptions options, OpenApiReference schemeReference) => options.AddSecurityScheme(SecuritySchemeType.Http, schemeReference, scheme => scheme.Scheme = "bearer");

    /// <summary>
    /// Adds a security scheme for OAuth2.
    /// </summary>
    /// <param name="options">The options.</param>
    /// <param name="flows">An object containing configuration information for the flow types supported.</param>
    /// <returns>The input options.</returns>
    public static OpenApiSecurityScheme AddOAuth2(this OpenApiOptions options, OpenApiOAuthFlows flows) => options.AddOAuth2(nameof(SecuritySchemeType.OAuth2), flows);

    /// <summary>
    /// Adds a security scheme for OAuth2.
    /// </summary>
    /// <param name="options">The options.</param>
    /// <param name="name">The name of the reference.</param>
    /// <param name="flows">An object containing configuration information for the flow types supported.</param>
    /// <returns>The input options.</returns>
    public static OpenApiSecurityScheme AddOAuth2(this OpenApiOptions options, string name, OpenApiOAuthFlows flows) => options.AddOAuth2(new OpenApiReference { Id = name, Type = ReferenceType.SecurityScheme }, flows);

    /// <summary>
    /// Adds a security scheme for OAuth2.
    /// </summary>
    /// <param name="options">The options.</param>
    /// <param name="schemeReference">The scheme reference.</param>
    /// <param name="flows">An object containing configuration information for the flow types supported.</param>
    /// <returns>The input options.</returns>
    public static OpenApiSecurityScheme AddOAuth2(this OpenApiOptions options, OpenApiReference schemeReference, OpenApiOAuthFlows flows) => options.AddSecurityScheme(SecuritySchemeType.OAuth2, schemeReference, scheme => scheme.Flows = flows);

    /// <summary>
    /// Adds a security scheme for OpenID Connect.
    /// </summary>
    /// <param name="options">The options.</param>
    /// <param name="openIdConnectUrl">The OpenID connection URL.</param>
    /// <returns>The input options.</returns>
    public static OpenApiSecurityScheme AddOpenIdConnect(this OpenApiOptions options, string openIdConnectUrl) => options.AddOpenIdConnect(openIdConnectUrl, nameof(SecuritySchemeType.OpenIdConnect));

    /// <summary>
    /// Adds a security scheme for OpenID Connect.
    /// </summary>
    /// <param name="options">The options.</param>
    /// <param name="openIdConnectUrl">The OpenID connection URL.</param>
    /// <param name="name">The name of the reference.</param>
    /// <returns>The input options.</returns>
    public static OpenApiSecurityScheme AddOpenIdConnect(this OpenApiOptions options, string openIdConnectUrl, string name) =>
        options.AddOpenIdConnect(openIdConnectUrl, new OpenApiReference { Id = name, Type = ReferenceType.SecurityScheme });

    /// <summary>
    /// Adds a security scheme for OpenID Connect.
    /// </summary>
    /// <param name="options">The options.</param>
    /// <param name="openIdConnectUrl">The OpenID connection URL.</param>
    /// <param name="schemeReference">The scheme reference.</param>
    /// <returns>The input options.</returns>
    public static OpenApiSecurityScheme AddOpenIdConnect(this OpenApiOptions options, string openIdConnectUrl, OpenApiReference schemeReference) =>
        options.AddSecurityScheme(SecuritySchemeType.OpenIdConnect, schemeReference, scheme => scheme.OpenIdConnectUrl = new Uri(openIdConnectUrl));

    /// <summary>
    /// Adds the specified security scheme.
    /// </summary>
    /// <param name="options">The options.</param>
    /// <param name="securityScheme">The security scheme.</param>
    /// <returns>The input options.</returns>
    public static OpenApiOptions AddSecurityScheme(this OpenApiOptions options, OpenApiSecurityScheme securityScheme) => options.AddDocumentTransformer(
        (document, _, _) =>
        {
            document.Components ??= new OpenApiComponents();
            document.Components.SecuritySchemes.Add(securityScheme.Reference.Id, securityScheme);

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
                RemoveAll(operation.Parameters, operation.Parameters.IntersectBy(parameterNamesToRemove, p => p.Name).ToArray(), cancellationToken);

                static void RemoveAll<T>(IList<T> values, IEnumerable<T> toRemove, CancellationToken cancellationToken)
                {
                    foreach (var remove in toRemove)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        _ = values.Remove(remove);
                    }
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
        options.WithAuthorizeCheck(() => new OpenApiSecurityScheme { Reference = new OpenApiReference { Id = name } });

    /// <summary>
    /// Adds a check for <see cref="Authorization.IAuthorizeData"/> and adds the specified scheme.
    /// </summary>
    /// <param name="options">The options.</param>
    /// <param name="schemeReference">The security scheme reference.</param>
    /// <returns>The input options.</returns>
    public static OpenApiOptions WithAuthorizeCheck(this OpenApiOptions options, OpenApiReference schemeReference) =>
        options.WithAuthorizeCheck(() => new OpenApiSecurityScheme { Reference = schemeReference });

    /// <summary>
    /// Adds a check for <see cref="Authorization.IAuthorizeData"/> and adds the specified scheme.
    /// </summary>
    /// <param name="options">The options.</param>
    /// <param name="securityScheme">The authentication scheme.</param>
    /// <returns>The input options.</returns>
    public static OpenApiOptions WithAuthorizeCheck(this OpenApiOptions options, OpenApiSecurityScheme? securityScheme) =>
        options.WithAuthorizeCheck(() => securityScheme);

    private static OpenApiOptions WithAuthorizeCheck(this OpenApiOptions options, Func<OpenApiSecurityScheme?> securitySchemeFactory) => options.AddOperationTransformer(
        (operation, context, _) =>
        {
            if (context.Description.ActionDescriptor.EndpointMetadata.Any(m => m is Microsoft.AspNetCore.Authorization.IAuthorizeData))
            {
                operation.Responses.AddOrUpdate(UnauthorizedKey, () => new() { Description = UnauthorizedValue }, r => r.Description = UnauthorizedValue);
                operation.Responses.AddOrUpdate(ForbiddenKey, () => new() { Description = ForbiddenValue }, r => r.Description = ForbiddenValue);

                if (securitySchemeFactory() is { } securityScheme)
                {
                    IList<string> requirements = [];

                    if (securityScheme.Type is SecuritySchemeType.OAuth2 or SecuritySchemeType.OpenIdConnect)
                    {
                        // gets any scopes
                        requirements = context.Description.ActionDescriptor.EndpointMetadata
                            .OfType<Microsoft.AspNetCore.Http.Metadata.IScopesMetadata>()
                            .Aggregate(
                                new List<string>(),
                                (scopes, metadata) =>
                                {
                                    scopes.AddRange(metadata.Scopes);
                                    return scopes;
                                });
                    }

                    operation.Security.Add(new() { [securityScheme] = requirements });
                }

                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        });

    private static OpenApiSecurityScheme AddSecurityScheme(this OpenApiOptions options, SecuritySchemeType type, OpenApiReference schemeReference, Action<OpenApiSecurityScheme>? configure = default)
    {
        var securityScheme = new OpenApiSecurityScheme
        {
            Type = type,
            Reference = schemeReference,
        };

        configure?.Invoke(securityScheme);
        _ = options.AddSecurityScheme(securityScheme);
        return securityScheme;
    }

    private static void AddOrUpdate(this OpenApiResponses responses, string key, Func<OpenApiResponse> addFunc, Action<OpenApiResponse> updateFunc)
    {
        if (responses.TryGetValue(key, out var value))
        {
            updateFunc(value);
            return;
        }

        responses.Add(key, addFunc());
    }
}