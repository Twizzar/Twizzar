using Microsoft.CodeAnalysis;

using Twizzar.Design.CoreInterfaces.Resources;
using Twizzar.SharedKernel.CoreInterfaces.Extensions;
using Twizzar.SharedKernel.NLog.Interfaces;
using Twizzar.SharedKernel.NLog.Logging;

using ViCommon.Functional;
using ViCommon.Functional.Monads.ResultMonad;

namespace Twizzar.Design.Infrastructure.VisualStudio.Roslyn
{
    /// <summary>
    /// Extension method for the <see cref="Workspace"/>.
    /// </summary>
    public static class WorkspaceExtensions
    {
        private static readonly object Lock = new object();

        /// <summary>
        /// Try to replace a node and apply the changes to the <see cref="Workspace"/> if it fails twice Log an error.
        /// </summary>
        /// <param name="workspace">The workspace.</param>
        /// <param name="oldRoot">Old root node.</param>
        /// <param name="newRoot">The new root node.</param>
        /// <returns>Success if successful else failure.</returns>
        public static IResult<Unit, Failure> ReplaceNode(this Workspace workspace, SyntaxNode oldRoot, SyntaxNode newRoot)
        {
            var callerContext = CallerContext.Create();

            lock (Lock)
            {
                for (int i = 0; i < 2; i++)
                {
                    var isSuccessful = workspace.CurrentSolution.GetDocumentIdsWithFilePath(oldRoot.SyntaxTree.FilePath).FirstOrNone()
                        .Map(documentId => workspace.CurrentSolution.GetDocument(documentId))
                        .Map(document => document.WithSyntaxRoot(newRoot))
                        .Match(
                            newDocument => workspace.TryApplyChanges(newDocument.Project.Solution),
                            () =>
                            {
                                ViLog.Log(callerContext, "Cannot get the document id form syntax tree.", LogLevel.Error);
                                return false;
                            });

                    if (isSuccessful)
                    {
                        return Result.Success<Unit, Failure>(Unit.New);
                    }

                    ViLog.Log(callerContext, $"Cannot update config after {i} retries", LogLevel.Error);
                }

                return Result.Failure<Unit, Failure>(
                    new Failure(
                        MessagesDesign.WorkspaceExtensions_ReplaceNode_Configuration_file_cannot_be_updated__Please_close_and_reopen_the_twizzar_PeekView_to_try_again_));
            }
        }
    }
}
