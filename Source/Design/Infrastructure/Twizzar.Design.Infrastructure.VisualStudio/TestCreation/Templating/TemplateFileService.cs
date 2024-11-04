using System.Diagnostics.CodeAnalysis;
using Twizzar.Design.CoreInterfaces.TestCreation.Templating;
using Twizzar.Design.Infrastructure.VisualStudio.TestCreation.BaseConfig;

namespace Twizzar.Design.Infrastructure.VisualStudio.TestCreation.Templating;

/// <summary>
/// Service interface for accessing a file in local file system.
/// </summary>
[ExcludeFromCodeCoverage]
public class TemplateFileService : BaseFileService, ITemplateFileService
{
    #region properties

    /// <summary>
    /// Gets the default template file content.
    /// </summary>
    protected override string Default =>
        """
        !! Code Template
        
        !! These template is setup for the NUnit test framework.
        !! see https://github.com/Twizzar/Twizzar/tree/main/defaultConfigs for templates that can be used as a starting point for different test frameworks.
        
        !! Purpose:
        !! This file serves as a template for generating the content of a unit test. 
        !! It contains placeholders and code snippets that can be customized to create comprehensive unit tests.
        
        !! Usage:
        !! - Utilize the code snippets and sections to structure your unit test as needed.
        !! - When <MyTag> is used in a code snipped it will be replaced with the code written under the [MyTag:] section.
        !! - Some tags are dynamic and will be replaced by another tag depending on the context.
        !!   - if a dynamic tag isn't defined, an internal default value is used
        !!   - <arrange> will be replaced by:
        !!      <sut-arrange>   if the type under test is non-static.
        !!      nothing         if the type under test is static.
        !!
        !!   - <arguments-arrange> will be replace by:
        !!      <method-arguments-arrange>      if the member under test is a method.
        !!      <property-field-setter-arrange> if the member under test is a property or field.
        !!
        !!   - <act> will be replaced by:
        !!      <void-method-act>                   if the member under test is a non-static void method.
        !!      <non-void-method-act>               if the member under test is a non-static non-void method.
        !!      <static-void-method-act>            if the member under test is a static void method.
        !!      <static-non-void-method-act>        if the member under test is a static non-void method.
        !!      <property-field-getter-act>         if the member under test is a property without a setter.
        !!      <property-field-setter-act>         if the member under test is a property with a setter.
        !!      <async-method-act>                  if the member under test is an async method that returns a task without result.
        !!      <async-result-method-act>           if the member under test is an async method that returns a task with result.
        !!      <static-async-method-act>           if the member is a static async method that returns a task without result.
        !!      <static-async-result-method-act>    if the member is a static async method that returns a task with result.
        
        !! The root of the unit test file.
        [test-file:]
        <test-usings>

        namespace $testNamespace$
        {
            <test-class>
        }

        [test-usings:]
        using NUnit.Framework;
        using Twizzar.Fixture;
        <argument-usings>

        !! This statement will be repeated for every dynamic usings. Dynamic usings are usings that are added based on the context.
        [argument-usings:]
        using $argumentNamespace$;

        [test-class:]
        [TestFixture]
        public class $testClass$
        {
            <test-method>
        }

        [test-method:]
        [Test]
        [TestSource(nameof($typeUnderTest$.$memberUnderTest$))]
        <method-signature>
        {
            // Arrange
            <arrange>
            <arguments-arrange>
            
            // Act
            <act>
            
            // Assert
            Assert.Fail();
        }

        [void-method-signature:]
        public void $testMethod$()
        
        [async-method-signature:]
        public async Task $testMethod$()
        
        !! <arrange>
        
        [sut-arrange:]
        var sut = new ItemBuilder<$typeUnderTest$>().Build();

        
        !! <arguments-arrange>
        
        [method-arguments-arrange:]
        var $argumentName$ = new ItemBuilder<$argumentType$>().Build();

        [property-field-setter-arrange:]
        var valueSet = new ItemBuilder<$argumentType$>().Build();

        
        !! <act>
        
        [void-method-act:]
        sut.$memberUnderTest$($argumentNames$);

        [non-void-method-act:]
        var returnValue = sut.$memberUnderTest$($argumentNames$);
        
        [async-method-act:]
        await sut.$memberUnderTest$($argumentNames$);
        
        [async-result-method-act:]
        var returnValue = await sut.$memberUnderTest$($argumentNames$);

        [static-async-method-act:]
        await $typeUnderTest$.$memberUnderTest$($argumentNames$);
        
        [static-async-result-method-act:]
        var returnValue = await $typeUnderTest$.$memberUnderTest$($argumentNames$);
        
        [static-void-method-act:]
        $typeUnderTest$.$memberUnderTest$($argumentNames$);

        [static-non-void-method-act:]
        var returnValue = $typeUnderTest$.$memberUnderTest$($argumentNames$);

        [property-field-getter-act:]
        var returnValue = sut.$memberUnderTest$;

        [property-field-setter-act:]
        sut.$memberUnderTest$ = valueSet;
        """;

    #endregion
}