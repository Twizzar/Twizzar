namespace Twizzar.Design.CoreInterfaces.TestCreation.Templating;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable SA1602 // Enumeration items should be documented

/// <summary>
/// Represents a snippet type.
/// </summary>
public enum SnippetType
{
    Default,
    File,
    Namespace,
    Class,
    Method,
    ArgumentUsing,
    Arrange,
    ArgumentArrange,
    Act,
    Warning,
    SutArrange,
    MethodArgumentArrange,
    PropertyFieldSetterArranger,
    VoidMethodAct,
    NonVoidMethodAct,
    StaticVoidMethodAct,
    StaticNonVoidMethodAct,
    PropertyFieldGetterAct,
    PropertyFieldSetterAct,
    MethodSignature,
    VoidMethodSignature,
    AsyncMethodSignature,
    AsyncMethodAct,
    AsyncResultMethodAct,
    StaticAsyncMethodAct,
    StaticAsyncResultMethodAct,
}