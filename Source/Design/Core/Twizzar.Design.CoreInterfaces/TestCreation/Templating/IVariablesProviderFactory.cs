namespace Twizzar.Design.CoreInterfaces.TestCreation.Templating;

/// <summary>
/// Factory for creating a<see cref="IVariablesProvider"/>.
/// </summary>
public interface IVariablesProviderFactory
{
    /// <summary>
    /// Creating a <see cref="IVariablesProvider"/>.
    /// </summary>
    /// <param name="templateContext"></param>
    /// <returns></returns>
    IVariablesProvider Create(ITemplateContext templateContext);
}