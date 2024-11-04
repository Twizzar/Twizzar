using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Twizzar.Design.CoreInterfaces.Query.Services;
using Twizzar.Design.Shared.CoreInterfaces.Name;
using Twizzar.Design.Ui.Helpers;
using Twizzar.Design.Ui.Interfaces.Parser.SyntaxTree;
using Twizzar.Design.Ui.Interfaces.Parser.SyntaxTree.Link;
using Twizzar.Design.Ui.Parser.SyntaxTree;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.EnsureHelper.Extensions;
using ViCommon.Functional.Monads.MaybeMonad;
using ViCommon.Functional.Monads.ResultMonad;

namespace Twizzar.Design.Ui.Validator;

/// <summary>
/// Type validator for generic method return types.
/// </summary>
public class GenericTypeValidator : BaseValidator
{
    #region fields

    private readonly ICompilationTypeQuery _compilationTypeQuery;
    private readonly IAssignableTypesQuery _assignableTypesQuery;
    private readonly IShortTypesConverter _shortTypesConverter;

    private readonly string[] _defaultValidInputs =
    {
        KeyWords.Null,
        KeyWords.Undefined,
    };

    private bool _isInitialized;

    #endregion

    #region ctors

    /// <summary>
    /// Initializes a new instance of the <see cref="GenericTypeValidator"/> class.
    /// </summary>
    /// <param name="typeDescription"></param>
    /// <param name="compilationTypeQuery"></param>
    /// <param name="assignableTypesQuery"></param>
    /// <param name="shortTypesConverter"></param>
    public GenericTypeValidator(
        IBaseDescription typeDescription,
        ICompilationTypeQuery compilationTypeQuery,
        IAssignableTypesQuery assignableTypesQuery,
        IShortTypesConverter shortTypesConverter)
        : base(typeDescription)
    {
        this.EnsureMany()
            .Parameter(typeDescription, nameof(typeDescription))
            .Parameter(compilationTypeQuery, nameof(compilationTypeQuery))
            .Parameter(assignableTypesQuery, nameof(assignableTypesQuery))
            .Parameter(shortTypesConverter, nameof(shortTypesConverter))
            .ThrowWhenNull();

        this._compilationTypeQuery = compilationTypeQuery;
        this._assignableTypesQuery = assignableTypesQuery;
        this._shortTypesConverter = shortTypesConverter;
        this.ValidInput = this._defaultValidInputs;

        Task.Run(this.InitializeAsync);
    }

    #endregion

    #region properties

    /// <inheritdoc />
    public override string DefaultValue => KeyWords.Undefined;

    #endregion

    #region members

    /// <inheritdoc />
    public override async Task InitializeAsync()
    {
        var assignableTypes =
            await this._assignableTypesQuery.GetAssignableTypesAsync(this._compilationTypeQuery.ObjectTypeDescription);

        this.ValidInput =
            this._defaultValidInputs.Concat(
                assignableTypes
                    .Select(description => description.TypeFullName.GetFriendlyCSharpTypeFullName()));

        this._isInitialized = true;
        this.Initialized();
    }

    /// <inheritdoc />
    public override IViToken Prettify(IViToken token) =>
        token switch
        {
            IViLinkToken link => this.Prepare(link),
            _ => base.Prettify(token),
        };

    /// <inheritdoc />
    protected override Task<IViToken> ValidateAsync(IViToken token) =>
        token switch
        {
            IViLinkToken linkToken => Task.FromResult(this.FindType(linkToken)),
            _ => Task.FromResult(token),
        };

    private IViToken FindType(IViLinkToken linkToken)
    {
        if (!this._isInitialized)
        {
            return new ViInvalidToken(
                linkToken.Start,
                linkToken.Length,
                linkToken.ContainingText,
                "Type system is still loading please wait.");
        }

        if (linkToken.TypeToken.AsMaybeValue() is SomeValue<IViTypeToken> typeToken)
        {
            return this.Find(typeToken.Value.TypeFullNameToken, typeToken.Value)
                .MapSuccess(token =>
                    (IViToken)linkToken.WithTypeToken(typeToken.Value.WithToken(token)))
                .Match(
                    token => token,
                    failure => new ViInvalidToken(
                        linkToken.Start,
                        linkToken.Length,
                        linkToken.ContainingText,
                        failure.Message));
        }
        else
        {
            return linkToken;
        }
    }

    private IResult<ITypeFullNameToken, Failure> Find(ITypeFullNameToken token, IViTypeToken typeToken) =>
        GetValid(this.FindTypes(token), typeToken)
            .Bind(typeResult =>
            {
                var tArguments = token.GenericTypeArguments
                    .Select(nameToken => this.Find(nameToken, typeToken))
                    .ToList();

                if (tArguments.All(t => t.IsSuccess))
                {
                    return
                        Result.Success<ITypeFullNameToken, Failure>(
                            typeResult.Token.WithGenericParameters(tArguments.Successes()));
                }

                return tArguments.First(r => r.IsFailure);
            });

    private IEnumerable<(string TypeFullName, ITypeFullNameToken Token)> FindTypes(
        ITypeFullNameToken token)
    {
        if (this._shortTypesConverter.IsShortType(token.ContainingText.Trim()))
        {
            var typeFullName = this._shortTypesConverter.ConvertToTypeFullName(token.ContainingText);

            return Enumerable.Empty<(string TypeFullName, ITypeFullNameToken Token)>()
                .Append((typeFullName, TypeFullName.Create(typeFullName).TypeFullNameToken));
        }

        var fullName = token.ToFullName();
        var typename = token.ToNameWithGenericPostfix();
        var ns = token.Namespace;

        return this._compilationTypeQuery
            .FindTypes(x => x.StartsWith(ns) && (x == fullName || x.EndsWith($".{typename}")))
            .Select(result => (result.MetadataName, result.Description.TypeFullName.GetTypeFullNameToken()));
    }

    private static IResult<(string TypeFullName, ITypeFullNameToken Token), Failure> GetValid(
        IEnumerable<(string TypeFullName, ITypeFullNameToken Token)> results,
        IViTypeToken typeToken) =>
        results.ToList() switch
        {
            { Count: 1 } x =>
                Result.Success<(string TypeFullName, ITypeFullNameToken Token), Failure>(x.Single()),
            { Count: > 1 } =>
                Result.Failure<(string TypeFullName, ITypeFullNameToken Token), Failure>(
                    new Failure(
                        $"The name {typeToken.ContainingText} is ambiguous please select one in the combobox.")),
            _ => Result.Failure<(string TypeFullName, ITypeFullNameToken Token), Failure>(
                new Failure($"No typ found with the name {typeToken.ContainingText}.")),
        };

    private IViToken Prepare(IViLinkToken linkToken)
    {
        if (!this._isInitialized && linkToken.TypeToken.AsMaybeValue() is SomeValue<IViTypeToken> someTypeToken)
        {
            var typeToken = someTypeToken.Value;

            var typeName = typeToken.TypeFullNameToken.ToFriendlyCSharpTypeName();

            typeToken =
                typeToken.With(typeToken.Start, typeToken.Start + typeName.Length, typeName) as IViTypeToken;

            return linkToken.WithTypeToken(typeToken);
        }

        return linkToken;
    }

    #endregion
}