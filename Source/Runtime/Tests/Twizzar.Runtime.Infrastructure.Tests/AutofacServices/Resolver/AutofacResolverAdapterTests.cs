using System;
using FluentAssertions;
using NUnit.Framework;
using Twizzar.Fixture;
using Twizzar.Runtime.Infrastructure.AutofacServices.Resolver;

namespace Twizzar.Runtime.Infrastructure.Tests.AutofacServices.Resolver
{
    [Category("TwizzarInternal")]
    public class AutofacResolverAdapterTests
    {
        [Test]
        public void AutofacResolver_CtorRegistrationSourceIsNull_ThrowArgumentNullException()
        {
            // assert
            Verify.Ctor<AutofacResolverAdapter>()
                .ShouldThrowArgumentNullException();
        }

        [Test]
        public void AutofacResolver_ResolveNamedServiceNameIsNull_ThrowArgumentNullException()
        {
            // arrange 
            var resolver = new ItemBuilder<AutofacResolverAdapter>().Build();

            //act
            Action action = () => resolver.ResolveNamed<int>(null);

            // assert
            action.Should().Throw<ArgumentNullException>();
        }
    }
}