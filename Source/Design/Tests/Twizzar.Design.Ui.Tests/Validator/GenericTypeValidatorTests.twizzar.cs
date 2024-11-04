﻿// <auto-generated />

using System.Collections.Immutable;
using TwizzarInternal.Fixture;
using Twizzar.Design.Shared.CoreInterfaces.Name;

namespace Twizzar.Design.Ui.Tests.Validator
{
    partial class GenericTypeValidatorTests
    {
        private class TypeFullNameTokenBuilder : ItemBuilder<Twizzar.Design.Shared.CoreInterfaces.Name.TypeFullNameToken, TypeFullNameTokena9bdBuilderPaths>
        {
            public TypeFullNameTokenBuilder()
            {
                this.With(p => p.Ctor.ns.Value(""));
                this.With(p => p.Ctor.arrayBrackets.Value(ImmutableArray<string>.Empty));
                this.With(p => p.Ctor.genericParameters.Value(ImmutableArray<ITypeFullNameToken>.Empty));
            }

            public TypeFullNameTokenBuilder WithTypeName(string typeName)
            {
                this.With(p => p.Ctor.typeName.Value(typeName));
                this.With(p => p.Ctor.containingText.Value(typeName));
                return this;
            }
        }
    }
}