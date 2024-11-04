using System;
using System.Text;
using Microsoft.CodeAnalysis;
using Twizzar.Analyzer2022.App.Interfaces;
using Twizzar.Design.Shared.Infrastructure.Interfaces;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;

namespace Twizzar.Analyzer2022.App.SourceTextGenerators;

/// <inheritdoc />
public class SymbolService : ISymbolService
{
    #region members

    /// <inheritdoc />
    public bool IsSymbolAccessibleWithin(Compilation compilation, IBaseDescription memberDescription, ISymbol withing)
    {
        var memberSymbol = ((IRoslynTypeDescription)memberDescription.GetReturnTypeDescription())
            .GetTypeSymbol();

        var memberTypeName = new StringBuilder();

        try
        {
            memberTypeName.AppendLine(memberSymbol.ToString());
            return compilation.IsSymbolAccessibleWithin(memberSymbol, withing);
        }
        catch (Exception)
        {
            return false;
        }
    }

    #endregion
}