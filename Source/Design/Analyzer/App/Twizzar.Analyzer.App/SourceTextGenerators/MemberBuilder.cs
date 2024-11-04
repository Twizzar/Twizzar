using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using Microsoft.CodeAnalysis;

using Twizzar.Analyzer.Core.Interfaces;
using Twizzar.Analyzer.SourceTextGenerators;
using Twizzar.Design.Shared.CoreInterfaces.Name;
using Twizzar.Design.Shared.Infrastructure;
using Twizzar.Design.Shared.Infrastructure.Interfaces;
using Twizzar.Design.Shared.Infrastructure.PathTree;
using Twizzar.Design.Shared.Infrastructure.VisualStudio2019.Name;
using Twizzar.SharedKernel.CoreInterfaces.Extensions;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Name;
using Twizzar.SharedKernel.NLog.Logging;

using ViCommon.Functional.Monads.MaybeMonad;
using ViCommon.Functional.Monads.ResultMonad;

namespace Twizzar.Analyzer.Core.SourceTextGenerators;

/// <summary>
/// Class for building one Member in the paths class.
/// </summary>
public record MemberBuilder(
    IPathSourceTextMemberGenerator PathSourceTextMemberGenerator,
    IRoslynDescriptionFactory DescriptionFactory,
    HashSet<string> UsingStatements,
    HashSet<string> ReservedMembers,
    string FixtureItemTypeName,
    Maybe<IPathNode> Parent,
    bool GenerateFuturePaths,
    Compilation Compilation,
    ISymbol SourceType,
    string ParentInstance,
    string DeclaredType,
    in List<MemberVerificationInfo> MembersForVerification,
    CancellationToken CancellationToken = default)
{
    #region fields

    private readonly HashSet<string> _definedParameterlessMethods = new();

    #endregion

    #region members

    /// <summary>
    /// Generate a property member.
    /// </summary>
    /// <param name="propertyDescription"></param>
    /// <returns></returns>
    public string GenerateMembers(IPropertyDescription propertyDescription)
    {
        this.ReservedMembers.Add($"get_{propertyDescription.Name}");

        return this.GenerateMembers(
            propertyDescription,
            propertyDescription.Name,
            propertyDescription.Name,
            "Property",
            this.ParentInstance);
    }

    /// <summary>
    /// Generate a field member.
    /// </summary>
    /// <param name="fieldDescription"></param>
    /// <returns></returns>
    public string GenerateMembers(IFieldDescription fieldDescription) =>
        this.GenerateMembers(
            fieldDescription,
            fieldDescription.Name,
            fieldDescription.Name,
            "Field",
            this.ParentInstance);

    /// <summary>
    /// Generate a method member.
    /// </summary>
    /// <param name="methodDescription"></param>
    /// <returns></returns>
    public string GenerateMembers(IMethodDescription methodDescription)
    {
        var methodName =
            methodDescription.Name +
            methodDescription.GenericTypeArguments.Values
                .Select(t => t.Name)
                .ToDisplayString(string.Empty);

        if (methodDescription.DeclaredParameters.Length > 0)
        {
            var additional = string.Empty;

            if (!this._definedParameterlessMethods.Contains(methodName))
            {
                additional = this.GenerateMembers(
                    methodDescription,
                    methodDescription.Name,
                    methodName,
                    "Method",
                    this.ParentInstance,
                    true);
            }

            this._definedParameterlessMethods.Add(methodName);

            return additional +
                   this.GenerateMembers(
                       methodDescription,
                       methodDescription.Name,
                       methodDescription.UniqueName,
                       "Method",
                       this.ParentInstance);
        }
        else if (!this._definedParameterlessMethods.Contains(methodName))
        {
            this._definedParameterlessMethods.Add(methodName);

            return this.GenerateMembers(
                methodDescription,
                methodDescription.Name,
                methodName,
                "Method",
                this.ParentInstance);
        }

        return string.Empty;
    }

    /// <summary>
    /// Generate a parameter member.
    /// </summary>
    /// <param name="parameterDescription"></param>
    /// <returns></returns>
    public string GenerateMembers(IParameterDescription parameterDescription) =>
        this.GenerateMembers(
            parameterDescription,
            parameterDescription.Name,
            parameterDescription.Name,
            "CtorParam",
            "TzParent");

    private string GenerateMembers(
        IBaseDescription memberDescription,
        string memberName,
        string memberPathName,
        string pathType,
        string parentInstance,
        bool isDefaultMethod = false)
    {
        this.CancellationToken.ThrowIfCancellationRequested();

        try
        {
            var currentNode = this.GetCurrentNode(memberPathName);

            if (currentNode.IsNone && !this.GenerateFuturePaths)
            {
                return string.Empty;
            }

            var memberReturnDescription = this.GetMemberReturnDescription(memberDescription, currentNode);
            var propertyName = memberPathName.ToSourceVariableCodeFriendly().MakeUnique(this.ReservedMembers);
            var innerClassName = $"Tz_{propertyName}MemberPath".MakeUnique(this.ReservedMembers);
            var attribute = GetAttribute(memberName, propertyName);
            var memberTypeName = GetMemberTypeName(memberDescription, memberReturnDescription);

            var methodTypeParameters = string.Empty;
            switch (this.GetMethodTypeParameters(memberDescription).AsResultValue())
            {
                case FailureValue<Failure> failure:
                    this.Log(failure.Value.Message + $" at {memberReturnDescription.TypeFullName}");
                    return string.Empty;
                case SuccessValue<string> s:
                    methodTypeParameters = s;
                    break;
            }

            var documentation = GetDocumentation(memberDescription, pathType, isDefaultMethod);
            var body = GetClassBody(memberDescription, this.FixtureItemTypeName);

            if (memberDescription is IMethodDescription && currentNode.IsSome)
            {
                this.MembersForVerification.Add(new MemberVerificationInfo(
                    memberDescription,
                    $"{this.DeclaredType}.{innerClassName}"));
            }

            var accessModifier = string.Empty;

            switch (GetAccessModifierResult(memberReturnDescription).AsResultValue())
            {
                case FailureValue<Failure> failure:
                    this.Log(failure.Value.Message + $" at {memberReturnDescription.TypeFullName}");
                    return string.Empty;
                case SuccessValue<string> s:
                    accessModifier = s.Value;
                    break;
            }

            this.ReservedMembers.Add(propertyName);

            if (memberDescription.IsBaseType)
            {
                return @$"

{documentation}
{attribute}
{accessModifier} {innerClassName} {propertyName} => 
    new {innerClassName}({parentInstance});

    {accessModifier} class {innerClassName} : {pathType}BasetypeMemberPath<{this.FixtureItemTypeName}, {memberTypeName}>
    {{
        public {innerClassName}(MemberPath<{this.FixtureItemTypeName}> parent)
            : base(""{memberName}"", parent{methodTypeParameters})
        {{
        }}

        {body}
    }}
";
            }
            else
            {
                var property = $@"

{documentation}
{attribute}
{accessModifier} {innerClassName} {propertyName} => new {innerClassName}({parentInstance}{methodTypeParameters});";

                var innerBody = this.Parent
                    .Map(
                        _ => this.PathSourceTextMemberGenerator.GenerateMembers(
                            memberReturnDescription.GetReturnTypeDescription(),
                            currentNode,
                            this.FixtureItemTypeName,
                            this.UsingStatements,
                            new HashSet<string>(ApiNames.ReservedPathProviderMembers.Add(innerClassName)),
                            this.GenerateFuturePaths,
                            this.Compilation,
                            this.SourceType,
                            this.MembersForVerification,
                            $"{this.DeclaredType}.{innerClassName}",
                            false))
                    .SomeOrProvided(string.Empty);

                var methodParametersCtor = (memberDescription is IMethodDescription)
                    ? ", string[] genericTypeArguments = null, params TzParameter[] parameters"
                    : string.Empty;

                var methodParametersBase = (memberDescription is IMethodDescription)
                    ? ", genericTypeArguments, parameters"
                    : string.Empty;

                var inner = $@"
{accessModifier} class {innerClassName} : {pathType}MemberPath<{this.FixtureItemTypeName}, {memberTypeName}>
{{
    public {innerClassName}(MemberPath<{this.FixtureItemTypeName}> parent{methodParametersCtor}) : base(""{memberName}"", parent{methodParametersBase}) {{ }}

    {body}
    {innerBody}
}}";

                return $@"
{property}

{inner}";
            }
        }
        catch (Exception exp)
        {
            this.Log(exp);
            return string.Empty;
        }
    }

    private static IResult<string, Failure> GetAccessModifierResult(IBaseDescription memberReturnDescription)
    {
        var accessModifierResult = memberReturnDescription.GetReturnTypeDescription() switch
        {
            IRoslynTypeDescription x => RoslynHelper.GetAccessModifierToken(x.GetTypeSymbol()),
            _ => Result.Success<string, Failure>("public"),
        };

        return accessModifierResult;
    }

    private Result<string, Failure> GetMethodTypeParameters(IBaseDescription memberDescription)
    {
        var sb = new StringBuilder();

        if (memberDescription is IMethodDescription methodDescription)
        {
            sb.Append(", new string[] { " +
                      methodDescription.GenericTypeArguments
                          .Select(pair => $"\"{pair.Value.Name}\"")
                          .ToDisplayString(", ") +
                      "}");

            foreach (var declaredParameter in methodDescription.DeclaredParameters)
            {
                var type = RoslynHelper.GetParameterTypeName(declaredParameter, this.Compilation);

                if (type.AsResultValue() is not SuccessValue<string> typeName)
                {
                    return type.GetFailureUnsafe();
                }

                sb.Append(
                    @$", new TzParameter(""{declaredParameter.Name}"", ""{declaredParameter.TypeFullName.GetTypeName()}"", typeof({typeName.Value}))");
            }

            if (methodDescription.DeclaredParameters.Length <= 0)
            {
                sb.Append(", new TzParameter[0]");
            }
        }

        return sb.ToString();
    }

    private static string GetMemberTypeName(IBaseDescription memberDescription, IBaseDescription memberReturnDescription) =>
        memberDescription switch
        {
            IMethodDescription m when m.TypeFullName.FullName == "System.Void" =>
                "TzVoid, TzVoid",
            IMethodDescription { IsGeneric: true } =>
                $"{memberReturnDescription.GetFriendlyReturnTypeFullName()}, object",
            IMethodDescription _ =>
                $"{memberReturnDescription.GetFriendlyReturnTypeFullName()}, {memberDescription.GetFriendlyReturnTypeFullName()}",
            _ => memberReturnDescription.GetFriendlyReturnTypeFullName(),
        };

    private static string GetAttribute(string memberName, string propertyName) =>
        propertyName != memberName
            ? @$"[OriginalName(""{memberName}"")]"
            : string.Empty;

    private static string GetClassBody(IBaseDescription memberDescription, string fixtureItemTypeName)
    {
        if (memberDescription is IMethodDescription methodDescription && methodDescription.TypeFullName.FullName != "System.Void")
        {
            var genericParam = string.Empty;
            var genericConstrain = string.Empty;

            var genericParameterMapping = methodDescription.GenericTypeArguments
                .Select(pair => pair.Value.Name)
                .ToDictionary(s => s, s => $"Tz{s}");

            var returnType = RoslynHelper.GetTypeNameWithMappedGenericParameters(methodDescription, genericParameterMapping);

            if (methodDescription.IsGeneric)
            {
                string GetName(ITypeFullName typeFullName)
                {
                    var symbol = ((SymbolTypeFullNameToken)typeFullName.Cast().Token).Symbol;
                    return RoslynHelper.GetTypeNameWithMappedGenericParameters(symbol, genericParameterMapping);
                }

                genericParam = genericParameterMapping.Values.ToDisplayString(", ", "<", ">");
                genericConstrain = methodDescription.GenericTypeArguments
                    .Where(pair => pair.Value.GetAllConstrainsAsString(name => name.FullName).Any())
                    .Select(pair =>
                        $"where {genericParameterMapping[pair.Value.Name]} : {pair.Value.GetAllConstrainsAsString(GetName).ToCommaSeparated()}")
                    .ToDisplayString(" ");
            }

            var parametersWithTailingComma = string.Empty;
            var parametersWithDiamond = string.Empty;
            var parameters = methodDescription.DeclaredParameters
                .Select(description => RoslynHelper.GetTypeNameWithMappedGenericParameters(description, genericParameterMapping))
                .ToCommaSeparated();

            if (parameters.Length > 0)
            {
                parametersWithTailingComma = parameters + ", ";
                parametersWithDiamond = $"<{parameters}>";
            }

            return $@"
/// <summary>
/// Setup the Method {EscapeXmlComment(methodDescription.ToString())} with a Function.
/// <param name=""methodFunction"">The function for calculating the method return value of type {EscapeXmlComment(returnType)}, with the parameters: {EscapeXmlComment(parameters)}.</param>
/// </summary>
public MemberConfig<{fixtureItemTypeName}> Value{genericParam}(Func<{parametersWithTailingComma}{returnType}> methodFunction) {genericConstrain} =>
    this.Delegate(methodFunction);

/// <summary>
/// Register a callback to the Method {EscapeXmlComment(methodDescription.ToString())}.
/// <remarks>When more than one callback is configured on the same member the last one is applied.</remarks>
/// <param name=""callback"">The callback action.</param>
/// </summary>
public MemberConfig<{fixtureItemTypeName}> Callback{genericParam}(Action{parametersWithDiamond} callback) {genericConstrain} =>
    this.RegisterCallback(callback);
";
        }
        else if (memberDescription is IPropertyDescription propertyDescription &&
                 (propertyDescription.DeclaredDescription?.IsInterface ?? false))
        {
            var returnType = propertyDescription.GetFriendlyReturnTypeFullName();

            return $@"
/// <summary>
/// Setup the Property {EscapeXmlComment(propertyDescription.ToString())} with a Function.
/// <param name=""propertyFunction"">The function for calculating the property return value of type {EscapeXmlComment(returnType)}.</param>
/// </summary>
public MemberConfig<{fixtureItemTypeName}> Value(Func<{returnType}> propertyFunction) =>
    this.Delegate(propertyFunction);
";
        }
        else
        {
            return string.Empty;
        }
    }

    private static string EscapeXmlComment(string comment) =>
        comment
            ?.Replace("<", " &lt;")
            ?.Replace(">", "&gt;");

    private IBaseDescription GetMemberReturnDescription(
        IBaseDescription memberDescription,
        Maybe<IPathNode> currentNode)
    {
        // if stub<T> or InstanceOf<T> was used.
        if (currentNode.AsMaybeValue() is SomeValue<IPathNode> pathNode &&
            pathNode.Value.TypeSymbol.AsMaybeValue() is SomeValue<ITypeSymbol>
            {
                Value: not IErrorTypeSymbol,
            } typeSymbol)
        {
            return this.DescriptionFactory.CreateDescription(typeSymbol.Value);
        }
        else if (memberDescription is IMethodDescription { IsGeneric: true })
        {
            return this.DescriptionFactory.CreateDescription(this.Compilation.ObjectType);
        }
        else
        {
            return memberDescription;
        }
    }

    private Maybe<IPathNode> GetCurrentNode(string memberPathName)
    {
        var currentNode = this.Parent.Bind(node => node.Children.GetMaybe(memberPathName));

        if (currentNode.IsNone)
        {
            // try again but this time try to convert the member back to its original name if possible.
            currentNode = this.Parent.Bind(node => node.Children.GetMaybe(memberPathName
                .ToSourceVariableCodeFriendly()
                .MakeUnique(this.ReservedMembers)));
        }

        return currentNode;
    }

    private static string GetDocumentation(IBaseDescription memberDescription, string pathType, bool isDefaultMethod)
    {
        var remark = string.Empty;

        if (isDefaultMethod)
        {
            remark =
                "<remarks>The default method selection always corresponds to the method overload with the least parameters.</remarks>";
        }

        return @$"
/// <summary>
/// Selection for the {EscapeXmlComment(pathType)}: {EscapeXmlComment(memberDescription.ToString())}.
/// {remark}
/// </summary>
";
    }

    #endregion
}