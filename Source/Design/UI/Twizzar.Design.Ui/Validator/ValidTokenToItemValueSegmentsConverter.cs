using System.Collections.Generic;
using Twizzar.Design.Ui.Interfaces.Enums;
using Twizzar.Design.Ui.Interfaces.Parser.SyntaxTree;
using Twizzar.Design.Ui.Interfaces.Parser.SyntaxTree.Link;
using Twizzar.Design.Ui.Interfaces.Parser.SyntaxTree.Literals;
using Twizzar.Design.Ui.Interfaces.Validator;
using Twizzar.Design.Ui.Interfaces.ValueObjects;
using Twizzar.SharedKernel.NLog.Interfaces;
using ViCommon.EnsureHelper;
using ViCommon.Functional.Monads.MaybeMonad;

#pragma warning disable SA1509 // Opening braces should not be preceded by blank line

namespace Twizzar.Design.Ui.Validator
{
    /// <inheritdoc />
    public class ValidTokenToItemValueSegmentsConverter : IValidTokenToItemValueSegmentsConverter
    {
        #region properties

        /// <inheritdoc />
        public ILogger Logger { get; set; }

        /// <inheritdoc />
        public IEnsureHelper EnsureHelper { get; set; }

        #endregion

        #region members

        /// <inheritdoc />
        public IEnumerable<ItemValueSegment> ToItemValueSegments(IViToken validatedToken) =>
            validatedToken switch
            {
                IViCtorToken x =>
                    CreateSingleValid(x.ContainingText, SegmentFormat.SelectedCtor),

                IViCodeToken x =>
                    CreateSingleInValid(x.ContainingText, SegmentFormat.ReadonlyCode),

                IViInvalidToken x =>
                    CreateSingleInValid(x.ContainingText, SegmentFormat.None),

                IViKeywordToken x =>
                    CreateSingleValid(x.ContainingText, SegmentFormat.Keyword),

                IViLinkToken viLinkToken =>
                    CreateLink(viLinkToken),

                IViBoolToken x =>
                    CreateSingleInValid(x.ContainingText, SegmentFormat.Boolean),

                IViCharToken x =>
                    CreateSingleInValid(x.ContainingText, SegmentFormat.Letter),

                IViNumericToken x =>
                    CreateSingleInValid(x.ContainingText, SegmentFormat.Number),

                IViStringToken x =>
                    CreateSingleInValid(x.ContainingText, SegmentFormat.Letter),

                IViEnumToken x =>
                    CreateSingleValid(x.ContainingText, SegmentFormat.Keyword),

                { } x => CreateSingleInValid(x.ContainingText, SegmentFormat.None),
            };

        private static IEnumerable<ItemValueSegment> CreateSingleValid(string content, SegmentFormat format) =>
            new[] { new ItemValueSegment(content, format, true) };

        private static IEnumerable<ItemValueSegment> CreateSingleInValid(string content, SegmentFormat format) =>
            new[] { new ItemValueSegment(content, format, false) };

        private static IEnumerable<ItemValueSegment> CreateLink(IViLinkToken linkToken)
        {
            if (linkToken.TypeToken.AsMaybeValue() is SomeValue<IViTypeToken> someType)
            {
                yield return new ItemValueSegment(someType.Value.ContainingText, SegmentFormat.Type, true);
            }
        }

        #endregion
    }
}