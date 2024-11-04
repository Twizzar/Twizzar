using System.Collections.Generic;
using FluentAssertions;
using Microsoft.CodeAnalysis.CSharp;
using NUnit.Framework;
using Twizzar.Design.Infrastructure.VisualStudio.Tests.Dummies;
using Twizzar.Design.Infrastructure.VisualStudio2019.Roslyn;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;
using Twizzar.TestCommon.TypeDescription.Builders;
using TwizzarInternal.Fixture;
using TypeFullName = Twizzar.Design.Shared.CoreInterfaces.Name.TypeFullName;

namespace Twizzar.Design.Infrastructure.VisualStudio2019.Tests.Roslyn;

public class CompilationTypeQueryTests
{
    [Test]
    public void Ctor_throws_when_argument_is_null()
    {
        Verify.Ctor<CompilationTypeQuery>()
            .SetupParameter("compilation", CSharpCompilation.Create(null))
            .ShouldThrowArgumentNullException();
    }

    [Test]
    public void ObjectTypeDescription_is_of_the_type_object()
    {
        var sut = new CompilationTypeQueryBuilder()
            .Build();

        sut.ObjectTypeDescription.TypeFullName.FullName.Should().Be("System.Object");
    }

    [Test]
    public void ValueTypeDescription_is_of_the_type_ValueType()
    {
        var sut = new CompilationTypeQueryBuilder()
            .Build();

        sut.ValueTypeDescription.TypeFullName.FullName.Should().Be("System.ValueType");
    }

    [Test]
    public void Type_get_found_from_cache()
    {
        var sut = new CompilationTypeQueryBuilder()
            .With(p => p.Ctor.compilationTypeCache.GetAllTypeForAssembly.Value(new List<ITypeDescription>()
            {
                new TypeDescriptionBuilder()
                    .WithTypeFullName(TypeFullName.CreateFromType(typeof(List<>))).Build()
            }))
            .Build();

        var results = sut.FindTypes(s => s.Contains("List"));
        var results2 = sut.FindTypes(typeResult => typeResult.MetadataName.Contains("List"));

        results.Should().HaveCount(1);
        results2.Should().HaveCount(1);
    }

    [Test]
    public void AllType_returns_cached_types()
    {
        var sut = new CompilationTypeQueryBuilder()
            .With(p => p.Ctor.compilationTypeCache.GetAllTypeForAssembly.Value(new List<ITypeDescription>()
            {
                new TypeDescriptionBuilder()
                    .WithTypeFullName(TypeFullName.CreateFromType(typeof(List<>)))
                    .Build(),
                new TypeDescriptionBuilder()
                    .WithTypeFullName(TypeFullName.CreateFromType(typeof(int)))
                    .Build(),
            }))
            .Build();

        var results = sut.AllTypes;

        results.Should().HaveCount(2);
    }

    private class CompilationTypeQueryBuilder : ItemBuilder<CompilationTypeQuery, CompilationTypeQueryPath>
    {
        /// <inheritdoc />
        public CompilationTypeQueryBuilder()
        {
            this.With(p => p.Ctor.compilation.Value(CSharpCompilation.Create(null)));
            this.With(p => p.Ctor.descriptionFactory.Value(new RoslynDescriptionFactoryDummy()));
        }
    }
}