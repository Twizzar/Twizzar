using Twizzar.Design.CoreInterfaces.Command.Services;

namespace Twizzar.Design.Core.Tests.Command.Services
{
    public class TestCommand : ITestCommand
    {
    }

    public interface ITestCommand : ICommand<ITestCommand>
    {

    }
}