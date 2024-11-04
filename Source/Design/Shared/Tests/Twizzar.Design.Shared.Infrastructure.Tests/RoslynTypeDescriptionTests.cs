using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;

using NUnit.Framework;

using Twizzar.Design.Infrastructure.VisualStudio.Tests.Builder;
using Twizzar.Design.Shared.Infrastructure.Description;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using Twizzar.SharedKernel.NLog.Logging;

namespace Twizzar.Design.Shared.Infrastructure.Tests
{

    public class RoslynTypeDescriptionTests
    {
        private Workspace _roslynWorkspace;
        private Project _project;
        private Compilation _compilation;

        [SetUp]
        public async Task SetUp()
        {
            this._roslynWorkspace = new RoslynWorkspaceBuilder()
                .AddReference(typeof(RoslynTypeDescription).Assembly.Location)
                .AddReference(typeof(RoslynTypeDescriptionTests).Assembly.Location)
                .AddReference(typeof(ITypeDescription).Assembly.Location)
                .AddReference(typeof(List<>).Assembly.Location)
                .Build();

            this._project = this._roslynWorkspace.CurrentSolution.Projects.FirstOrDefault();
            this._compilation = await this._project.GetCompilationAsync();

            LoggerFactory.SetConfig(new LoggerConfigurationBuilder());
        }
    }
}
