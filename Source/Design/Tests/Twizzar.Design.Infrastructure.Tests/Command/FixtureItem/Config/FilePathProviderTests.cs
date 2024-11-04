using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Twizzar.Design.CoreInterfaces.Common.VisualStudio;
using Twizzar.Design.Infrastructure.Command.FixtureItem.Config;
using Twizzar.TestCommon;
using TwizzarInternal.Fixture;
using ViCommon.Functional.Monads.MaybeMonad;

namespace Twizzar.Design.Infrastructure.Tests.Command.FixtureItem.Config
{
    [Category("TwizzarInternal")]
    public partial class FilePathProviderTests
    {
        private const string ProjectName = "projectName";
        private const string ProjectDir = "projectDir";

        [Test]
        public void Check_ctor_throws_null_exception()
        {
            Verify.Ctor<FilePathProvider>()
                .ShouldThrowArgumentNullException();
        }

        [Test]
        public async Task GetConfigFilePath_calls_vsProjectManager()
        {
            // arrange
            var vsProjectManager = Build.New<IVsProjectManager>();
            var filePathProvider = new FilePathProvider(vsProjectManager);

            // act
            var result = await filePathProvider.GetConfigFilePath(ProjectName);

            // assert
            result.IsSome.Should().BeFalse();
            Mock.Get(vsProjectManager)
                .Verify(mock => mock.GetProjectPath(ProjectName), Times.Once);
        }

        [Test]
        public async Task GetConfigFilePath_calls_vsProjectManager_and_combines_with_definitionFileName()
        {
            // arrange
            var vsProjectManager = new ItemBuilder<IVsProjectManager>()
                .With(p => p.GetProjectPath.Value(Task.FromResult(Maybe.Some(ProjectDir))))
                .Build();

            var filePathProvider = new FilePathProvider(vsProjectManager);
            
            // act
            var result = await filePathProvider.GetConfigFilePath(ProjectName);

            // assert
            result.IsSome.Should().BeTrue();
            Mock.Get(vsProjectManager).Verify(mock => mock.GetProjectPath(ProjectName), Times.Once);
            result.GetValueUnsafe().Should().Be(ProjectDir + @"\" + ProjectName + ".fixture.yaml");
        }
    }
}