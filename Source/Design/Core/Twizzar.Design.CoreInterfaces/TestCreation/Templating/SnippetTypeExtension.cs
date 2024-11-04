using System;
using System.Diagnostics.CodeAnalysis;

namespace Twizzar.Design.CoreInterfaces.TestCreation.Templating;

/// <summary>
/// Extension methods for the <see cref="SnippetType"/>.
/// </summary>
[ExcludeFromCodeCoverage]
public static class SnippetTypeExtension
{
    private const string TestFile = "test-file";
    private const string ArgumentUsings = "argument-usings";
    private const string Class = "test-class";
    private const string Arrange = "arrange";
    private const string ArgumentsArrange = "arguments-arrange";
    private const string Act = "act";
    private const string TestMethod = "test-method";
    private const string TestNamespace = "test-namespace";
    private const string SutArrange = "sut-arrange";
    private const string MethodArgumentArrange = "method-arguments-arrange";
    private const string PropertyFieldSetterArranger = "property-field-setter-arrange";
    private const string VoidMethodAct = "void-method-act";
    private const string NonVoidMethodAct = "non-void-method-act";
    private const string StaticVoidMethodAct = "static-void-method-act";
    private const string StaticNonVoidMethodAct = "static-non-void-method-ac";
    private const string PropertyFieldGetterAct = "property-field-getter-act";
    private const string PropertyFieldSetterAct = "property-field-setter-act";
    private const string MethodSignature = "method-signature";
    private const string VoidMethodSignature = "void-method-signature";
    private const string AsyncMethodSignature = "async-method-signature";
    private const string AsyncMethodAct = "async-method-act";
    private const string AsyncResultMethodAct = "async-result-method-act";
    private const string StaticAsyncMethodAct = "static-async-method-act";
    private const string StaticAsyncResultMethodAct = "static-async-result-method-act";

    /// <summary>
    /// Convert a type to a tag.
    /// </summary>
    /// <param name="type"></param>
    /// <returns>Tag without the delimiters.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// When the type is <see cref="SnippetType.Default"/> and <see cref="SnippetType.Warning"/>.
    /// </exception>
    public static string ToTag(this SnippetType type) =>
        type switch
        {
            SnippetType.File => TestFile,
            SnippetType.ArgumentUsing => ArgumentUsings,
            SnippetType.Class => Class,
            SnippetType.Arrange => Arrange,
            SnippetType.ArgumentArrange => ArgumentsArrange,
            SnippetType.Act => Act,
            SnippetType.Method => TestMethod,
            SnippetType.Namespace => TestNamespace,
            SnippetType.SutArrange => SutArrange,
            SnippetType.MethodArgumentArrange => MethodArgumentArrange,
            SnippetType.PropertyFieldSetterArranger => PropertyFieldSetterArranger,
            SnippetType.VoidMethodAct => VoidMethodAct,
            SnippetType.NonVoidMethodAct => NonVoidMethodAct,
            SnippetType.StaticVoidMethodAct => StaticVoidMethodAct,
            SnippetType.StaticNonVoidMethodAct => StaticNonVoidMethodAct,
            SnippetType.PropertyFieldGetterAct => PropertyFieldGetterAct,
            SnippetType.PropertyFieldSetterAct => PropertyFieldSetterAct,
            SnippetType.MethodSignature => MethodSignature,
            SnippetType.VoidMethodSignature => VoidMethodSignature,
            SnippetType.AsyncMethodSignature => AsyncMethodSignature,
            SnippetType.AsyncMethodAct => AsyncMethodAct,
            SnippetType.AsyncResultMethodAct => AsyncResultMethodAct,
            SnippetType.StaticAsyncMethodAct => StaticAsyncMethodAct,
            SnippetType.StaticAsyncResultMethodAct => StaticAsyncResultMethodAct,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null),
        };

    /// <summary>
    /// Convert a tag to a type.
    /// </summary>
    /// <param name="tag"></param>
    /// <returns></returns>
    public static SnippetType ToSnippedType(this string tag) =>
        tag switch
        {
            TestFile => SnippetType.File,
            Class => SnippetType.Class,
            ArgumentUsings => SnippetType.ArgumentUsing,
            Arrange => SnippetType.Arrange,
            ArgumentsArrange => SnippetType.ArgumentArrange,
            Act => SnippetType.Act,
            TestMethod => SnippetType.Method,
            TestNamespace => SnippetType.Namespace,
            SutArrange => SnippetType.SutArrange,
            MethodArgumentArrange => SnippetType.MethodArgumentArrange,
            PropertyFieldSetterArranger => SnippetType.PropertyFieldSetterArranger,
            VoidMethodAct => SnippetType.VoidMethodAct,
            NonVoidMethodAct => SnippetType.NonVoidMethodAct,
            StaticVoidMethodAct => SnippetType.StaticVoidMethodAct,
            StaticNonVoidMethodAct => SnippetType.StaticNonVoidMethodAct,
            PropertyFieldGetterAct => SnippetType.PropertyFieldGetterAct,
            PropertyFieldSetterAct => SnippetType.PropertyFieldSetterAct,
            MethodSignature => SnippetType.MethodSignature,
            VoidMethodSignature => SnippetType.VoidMethodSignature,
            AsyncMethodSignature => SnippetType.AsyncMethodSignature,
            AsyncMethodAct => SnippetType.AsyncMethodAct,
            AsyncResultMethodAct => SnippetType.AsyncResultMethodAct,
            StaticAsyncMethodAct => SnippetType.StaticAsyncMethodAct,
            StaticAsyncResultMethodAct => SnippetType.StaticAsyncResultMethodAct,
            _ => SnippetType.Default,
        };
}