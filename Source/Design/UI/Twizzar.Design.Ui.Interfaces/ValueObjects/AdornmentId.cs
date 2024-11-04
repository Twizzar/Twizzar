using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Twizzar.SharedKernel.CoreInterfaces;
using Twizzar.SharedKernel.CoreInterfaces.FunctionalParser;
using ViCommon.EnsureHelper;
using ViCommon.EnsureHelper.ArgumentHelpers.Extensions;
using ViCommon.Functional.Monads.ResultMonad;

namespace Twizzar.Design.Ui.Interfaces.ValueObjects
{
    /// <summary>
    /// The adornment id identifies the adornment. It can be used by the AdornmentExpander and the FixtureItemPanel to find out to which adornment they belong.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class AdornmentId : ValueObject
    {
        #region static fields and constants

        /// <summary>
        /// Gets the prefix of the unique adornment string.
        /// </summary>
        public const string Prefix = "ViAdornment";

        #endregion

        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="AdornmentId"/> class.
        /// </summary>
        /// <param name="value">The value of the id.</param>
        /// <param name="projectName"></param>
        private AdornmentId(Guid value, string projectName)
        {
            EnsureHelper.GetDefault.Parameter(projectName, nameof(projectName))
                .IsNotNullAndNotEmpty()
                .ThrowOnFailure();

            this.Value = value;
            this.ProjectName = projectName;
        }

        #endregion

        #region properties

        /// <summary>
        /// Gets the value of the id.
        /// </summary>
        public Guid Value { get; }

        /// <summary>
        /// Gets the project name.
        /// </summary>
        public string ProjectName { get; }

        private static ParserExtensions.Parser<AdornmentId> Parser =>
            input =>
                from head in Consume.String($"{Prefix} ")(input)
                from guid in Consume.CharExcept(char.IsWhiteSpace, "White space expected").Many()(head.OutputPoint)
                from sep in Consume.WhiteSpace(guid.OutputPoint)
                from projectName in Consume.AnyChar.Many()(sep.OutputPoint)
                from eof in Consume.EOF(projectName.OutputPoint)
                select ParseSuccess<AdornmentId>.FromSpan(
                    input,
                    eof.OutputPoint,
                    new AdornmentId(
                        Guid.Parse(guid.Value.AsString()),
                        projectName.Value.AsString()));

        #endregion

        #region members

        /// <summary>
        /// Implicit conversion to Guid.
        /// </summary>
        /// <param name="id"></param>
        public static implicit operator Guid(AdornmentId id) =>
            id.Value;

        /// <inheritdoc />
        public override string ToString() =>
            $"{Prefix} {this.Value} {this.ProjectName}";

        /// <summary>
        /// Create a new unique id.
        /// </summary>
        /// <param name="projectName">The project name.</param>
        /// <returns>The created adornment id.</returns>
        public static AdornmentId CreateNew(string projectName) =>
            new(Guid.NewGuid(), projectName);

        /// <summary>
        /// Creates the adornment id form a guid.
        /// </summary>
        /// <param name="guid">The guid.</param>
        /// <param name="projectName">The project name.</param>
        /// <returns>The created adornment id.</returns>
        public static AdornmentId CreateFrom(Guid guid, string projectName) =>
            new(guid, projectName);

        /// <summary>
        /// Parse a string in the format: <c>ViAdornment GUID PROJECT_NAME</c>.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <returns>Success when the parse was successful else <see cref="ParseFailure"/>.</returns>
        public static IResult<AdornmentId, ParseFailure> Parse(string input) =>
            Parser.Parse(input)
                .MapSuccess(success => success.Value);

        /// <inheritdoc />
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return this.Value;
            yield return this.ProjectName;
        }

        #endregion
    }
}