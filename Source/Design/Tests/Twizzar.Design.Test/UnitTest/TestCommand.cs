using Twizzar.Design.CoreInterfaces.Command.Services;

namespace Twizzar.Design.Test.UnitTest;

public class TestCommand : ITestCommand
{
}

public interface ITestCommand : ICommand<ITestCommand>
{

}