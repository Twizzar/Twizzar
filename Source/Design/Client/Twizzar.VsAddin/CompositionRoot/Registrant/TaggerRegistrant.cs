using System.Diagnostics.CodeAnalysis;

using Autofac;
using Twizzar.Design.CoreInterfaces.Adornment;
using Twizzar.Design.CoreInterfaces.Common.FixtureItem.Adornment;
using Twizzar.Design.Infrastructure.VisualStudio.FixtureItem.Adornment;
using Twizzar.Design.Infrastructure.VisualStudio.FixtureItem.Adornment.IntraTextAdornment;
using Twizzar.Design.Infrastructure.VisualStudio.Interfaces;
using Twizzar.Design.Infrastructure.VisualStudio.Roslyn;
using Twizzar.Design.Infrastructure.VisualStudio.Services;
using Twizzar.Design.Ui.Interfaces.VisualStudio;
using Twizzar.VsAddin.Factory;
using Twizzar.VsAddin.Interfaces.CompositionRoot;

namespace Twizzar.VsAddin.CompositionRoot.Registrant
{
    /// <summary>
    /// Registrant for the tagger related.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class TaggerRegistrant : IIocComponentRegistrant
    {
        #region Implementation of IIocComponentRegistrant

        /// <inheritdoc />
        public void RegisterComponents(ContainerBuilder builder)
        {
            builder.RegisterType<ViDocumentTaggerFactory>()
                .As<IViDocumentTaggerFactory>();

            builder.RegisterType<ViDocumentTagger>()
                .As<IViDocumentTagger>();

            builder.RegisterType<DocumentWorkspaceFactory>()
                .As<IDocumentWorkspaceFactory>()
                .SingleInstance();

            builder.RegisterType<RoslynDocumentReaderFactory>()
                .As<IRoslynDocumentReaderFactory>()
                .SingleInstance();

            builder.RegisterType<RoslynDocumentWriterFactory>()
                .As<IRoslynDocumentWriterFactory>()
                .SingleInstance();

            builder.RegisterType<RoslynDocumentReader>()
                .As<IDocumentReader>();

            builder.RegisterType<RoslynDocumentWriter>()
                .As<IDocumentWriter>();

            builder.RegisterType<AsyncTaskRunnerAdapter>()
                .As<IAsyncTaskRunner>()
                .SingleInstance();

            builder.RegisterType<DocumentWorkspace>()
                .As<IDocumentWorkspace>();

            builder.RegisterType<SnapshotHistory>()
                .As<ISnapshotHistory>();

            builder.RegisterType<ViAdornment>()
                .As<IViAdornment>();

            builder.RegisterType<ViAdornmentCacheCreator>()
                .As<IViAdornmentCache>();

            builder.RegisterType<ViAdornmentCreator>()
                .As<IViAdornmentCreator>();

            builder.RegisterType<DocumentAdornmentController>()
                .As<IDocumentAdornmentController>();
        }

        #endregion
    }
}