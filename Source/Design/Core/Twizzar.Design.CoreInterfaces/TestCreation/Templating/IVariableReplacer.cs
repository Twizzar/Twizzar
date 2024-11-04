namespace Twizzar.Design.CoreInterfaces.TestCreation.Templating;

/// <summary>
/// Service for replacing variable names with the respective value.
/// </summary>
public interface IVariableReplacer
{
    /// <summary>
    /// Replace all variable names by the values in the <see cref="IVariablesProvider"/>.
    /// </summary>
    /// <param name="code"></param>
    /// <param name="variablesProvider"></param>
    /// <returns></returns>
    string ReplaceAll(string code, IVariablesProvider variablesProvider);
}