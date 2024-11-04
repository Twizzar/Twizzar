using System.Collections.Generic;

namespace Twizzar.Design.CoreInterfaces.TestCreation.Templating;

/// <summary>
/// Provides all variables which can be used by the user.
/// This does not include:
/// - user defined variables with tags.
/// - loop variables for example argument usings.
/// </summary>
public interface IVariablesProvider : IEnumerable<TemplateVariable>
{
}