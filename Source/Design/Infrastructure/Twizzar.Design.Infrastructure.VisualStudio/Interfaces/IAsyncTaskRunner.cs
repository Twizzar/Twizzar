using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Threading;
using Twizzar.SharedKernel.CoreInterfaces;

namespace Twizzar.Design.Infrastructure.VisualStudio.Interfaces
{
    /// <summary>
    /// Interface for the <see cref="JoinableTaskFactory"/> class to run async tasks synchronously.
    /// </summary>
    public interface IAsyncTaskRunner : IService
    {
        /// <summary>
        /// Runs the specified asynchronous method to completion while synchronously blocking the calling thread.
        /// </summary>
        /// <param name="asyncMethod">The asynchronous method to execute.</param>
        /// <remarks>
        /// <para>Any exception thrown by the delegate is rethrown in its original type to the caller of this method.</para>
        /// <para>When the delegate resumes from a yielding await, the default behavior is to resume in its original context
        /// as an ordinary async method execution would. For example, if the caller was on the main thread, execution
        /// resumes after an await on the main thread; but if it started on a threadpool thread it resumes on a threadpool thread.</para>
        /// <example>
        /// <code>
        /// // On threadpool or Main thread, this method will block
        /// // the calling thread until all async operations in the
        /// // delegate complete.
        /// joinableTaskFactory.Run(async delegate {
        ///     // still on the threadpool or Main thread as before.
        ///     await OperationAsync();
        ///     // still on the threadpool or Main thread as before.
        ///     await Task.Run(async delegate {
        ///          // Now we're on a threadpool thread.
        ///          await Task.Yield();
        ///          // still on a threadpool thread.
        ///     });
        ///     // Now back on the Main thread (or threadpool thread if that's where we started).
        /// });
        /// </code>
        /// </example>
        /// </remarks>
        public void Run(Func<Task> asyncMethod);

        /// <summary>
        /// Runs the specified asynchronous method to completion while synchronously blocking the calling thread.
        /// </summary>
        /// <param name="asyncMethod">The asynchronous method to execute.</param>
        /// <param name="creationOptions">The <see cref="T:Microsoft.VisualStudio.Threading.JoinableTaskCreationOptions" /> used to customize the task's behavior.</param>
        public void Run(Func<Task> asyncMethod, JoinableTaskCreationOptions creationOptions);

        /// <summary>
        /// Runs the specified asynchronous method to completion while synchronously blocking the calling thread.
        /// </summary>
        /// <typeparam name="T">The type of value returned by the asynchronous operation.</typeparam>
        /// <param name="asyncMethod">The asynchronous method to execute.</param>
        /// <returns>The result of the Task returned by <paramref name="asyncMethod" />.</returns>
        /// <remarks>
        /// <para>Any exception thrown by the delegate is rethrown in its original type to the caller of this method.</para>
        /// <para>When the delegate resumes from a yielding await, the default behavior is to resume in its original context
        /// as an ordinary async method execution would. For example, if the caller was on the main thread, execution
        /// resumes after an await on the main thread; but if it started on a threadpool thread it resumes on a threadpool thread.</para>
        /// <para>See the <see cref="M:Microsoft.VisualStudio.Threading.JoinableTaskFactory.Run(System.Func{System.Threading.Tasks.Task})" /> overload documentation for an example.</para>
        /// </remarks>
        public T Run<T>(Func<Task<T>> asyncMethod);

        /// <summary>
        /// Runs the specified asynchronous method to completion while synchronously blocking the calling thread.
        /// </summary>
        /// <typeparam name="T">The type of value returned by the asynchronous operation.</typeparam>
        /// <param name="asyncMethod">The asynchronous method to execute.</param>
        /// <param name="creationOptions">The <see cref="T:Microsoft.VisualStudio.Threading.JoinableTaskCreationOptions" /> used to customize the task's behavior.</param>
        /// <returns>The result of the Task returned by <paramref name="asyncMethod" />.</returns>
        /// <remarks>
        /// <para>Any exception thrown by the delegate is rethrown in its original type to the caller of this method.</para>
        /// <para>When the delegate resumes from a yielding await, the default behavior is to resume in its original context
        /// as an ordinary async method execution would. For example, if the caller was on the main thread, execution
        /// resumes after an await on the main thread; but if it started on a threadpool thread it resumes on a threadpool thread.</para>
        /// </remarks>
        public T Run<T>(Func<Task<T>> asyncMethod, JoinableTaskCreationOptions creationOptions);

        /// <summary>
        /// Invokes an async delegate on the caller's thread, and yields back to the caller when the async method yields.
        /// The async delegate is invoked in such a way as to mitigate deadlocks in the event that the async method
        /// requires the main thread while the main thread is blocked waiting for the async method's completion.
        /// </summary>
        /// <param name="asyncMethod">The method that, when executed, will begin the async operation.</param>
        /// <returns>An object that tracks the completion of the async operation, and allows for later synchronous blocking of the main thread for completion if necessary.</returns>
        /// <remarks>
        /// <para>Exceptions thrown by the delegate are captured by the returned <see cref="T:Microsoft.VisualStudio.Threading.JoinableTask" />.</para>
        /// <para>When the delegate resumes from a yielding await, the default behavior is to resume in its original context
        /// as an ordinary async method execution would. For example, if the caller was on the main thread, execution
        /// resumes after an await on the main thread; but if it started on a threadpool thread it resumes on a threadpool thread.</para>
        /// </remarks>
        public JoinableTask RunAsync(Func<Task> asyncMethod);

        /// <summary>
        /// Invokes an async delegate on the caller's thread, and yields back to the caller when the async method yields.
        /// The async delegate is invoked in such a way as to mitigate deadlocks in the event that the async method
        /// requires the main thread while the main thread is blocked waiting for the async method's completion.
        /// </summary>
        /// <param name="asyncMethod">The method that, when executed, will begin the async operation.</param>
        /// <returns>An object that tracks the completion of the async operation, and allows for later synchronous blocking of the main thread for completion if necessary.</returns>
        /// <param name="creationOptions">The <see cref="T:Microsoft.VisualStudio.Threading.JoinableTaskCreationOptions" /> used to customize the task's behavior.</param>
        /// <remarks>
        /// <para>Exceptions thrown by the delegate are captured by the returned <see cref="T:Microsoft.VisualStudio.Threading.JoinableTask" />.</para>
        /// <para>When the delegate resumes from a yielding await, the default behavior is to resume in its original context
        /// as an ordinary async method execution would. For example, if the caller was on the main thread, execution
        /// resumes after an await on the main thread; but if it started on a threadpool thread it resumes on a threadpool thread.</para>
        /// </remarks>
        public JoinableTask RunAsync(
            Func<Task> asyncMethod,
            JoinableTaskCreationOptions creationOptions);

        /// <summary>
        /// Invokes an async delegate on the caller's thread, and yields back to the caller when the async method yields.
        /// The async delegate is invoked in such a way as to mitigate deadlocks in the event that the async method
        /// requires the main thread while the main thread is blocked waiting for the async method's completion.
        /// </summary>
        /// <typeparam name="T">The type of value returned by the asynchronous operation.</typeparam>
        /// <param name="asyncMethod">The method that, when executed, will begin the async operation.</param>
        /// <returns>
        /// An object that tracks the completion of the async operation, and allows for later synchronous blocking of the main thread for completion if necessary.
        /// </returns>
        /// <remarks>
        /// <para>Exceptions thrown by the delegate are captured by the returned <see cref="T:Microsoft.VisualStudio.Threading.JoinableTask" />.</para>
        /// <para>When the delegate resumes from a yielding await, the default behavior is to resume in its original context
        /// as an ordinary async method execution would. For example, if the caller was on the main thread, execution
        /// resumes after an await on the main thread; but if it started on a threadpool thread it resumes on a threadpool thread.</para>
        /// </remarks>
        public JoinableTask<T> RunAsync<T>(Func<Task<T>> asyncMethod);

        /// <summary>
        /// Invokes an async delegate on the caller's thread, and yields back to the caller when the async method yields.
        /// The async delegate is invoked in such a way as to mitigate deadlocks in the event that the async method
        /// requires the main thread while the main thread is blocked waiting for the async method's completion.
        /// </summary>
        /// <typeparam name="T">The type of value returned by the asynchronous operation.</typeparam>
        /// <param name="asyncMethod">The method that, when executed, will begin the async operation.</param>
        /// <param name="creationOptions">The <see cref="T:Microsoft.VisualStudio.Threading.JoinableTaskCreationOptions" /> used to customize the task's behavior.</param>
        /// <returns>
        /// An object that tracks the completion of the async operation, and allows for later synchronous blocking of the main thread for completion if necessary.
        /// </returns>
        /// <remarks>
        /// <para>Exceptions thrown by the delegate are captured by the returned <see cref="T:Microsoft.VisualStudio.Threading.JoinableTask" />.</para>
        /// <para>When the delegate resumes from a yielding await, the default behavior is to resume in its original context
        /// as an ordinary async method execution would. For example, if the caller was on the main thread, execution
        /// resumes after an await on the main thread; but if it started on a threadpool thread it resumes on a threadpool thread.</para>
        /// </remarks>
        public JoinableTask<T> RunAsync<T>(
            Func<Task<T>> asyncMethod,
            JoinableTaskCreationOptions creationOptions);
    }
}