using System.Diagnostics.CodeAnalysis;
using Twizzar.Design.CoreInterfaces.Command.Services;

namespace Twizzar.Design.Infrastructure.Tests.Command.Services
{
    [ExcludeFromCodeCoverage]
    // ReSharper disable once ClassNeverInstantiated.Global
    public class TestCommand : ITestCommand
    {
    }

    public interface ITestCommand : ICommand<ITestCommand>
    {

    }
}