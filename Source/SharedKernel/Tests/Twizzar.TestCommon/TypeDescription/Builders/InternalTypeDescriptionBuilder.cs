using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Moq;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Description;

namespace Twizzar.TestCommon.TypeDescription.Builders
{
    public class InternalTypeDescriptionBuilder<T> where T : class, ITypeDescription
    {
        #region fields

        private readonly Mock<T> _typeDescriptionMock;

        #endregion

        #region ctors

        public InternalTypeDescriptionBuilder(Mock<T> typeDescriptionMock)
        {
            this._typeDescriptionMock = typeDescriptionMock;
            this.DeclaredConstructors = Enumerable.Empty<IMethodDescription>();
            this.DeclaredProperties = Enumerable.Empty<IPropertyDescription>();
            this.DeclaredFields = Enumerable.Empty<IFieldDescription>();
            this.DeclaredMethod = Enumerable.Empty<IMethodDescription>();
        }


        #endregion

        #region properties

        private IEnumerable<IMethodDescription> DeclaredConstructors { get; set; }

        private IEnumerable<IPropertyDescription> DeclaredProperties { get; set; }

        private IEnumerable<IFieldDescription> DeclaredFields { get; set; }

        public IEnumerable<IMethodDescription> DeclaredMethod { get; set; }

        #endregion

        #region members

        public InternalTypeDescriptionBuilder<T> WithDeclaredConstructors(params IMethodDescription[] ctors)
        {
            this.DeclaredConstructors = ctors;
            return this;
        }

        public InternalTypeDescriptionBuilder<T> WithDeclaredConstructorsParams(IEnumerable<string> names)
        {
            IEnumerable<IParameterDescription> ParamDescriptions()
            {
                foreach (var name in names)
                {
                    var m = new Mock<IParameterDescription>();

                    m.Setup(description => description.Name)
                        .Returns(name);

                    yield return m.Object;
                }
            }

            var methodDesc = new Mock<IMethodDescription>();

            methodDesc.Setup(description => description.DeclaredParameters)
                .Returns(ParamDescriptions().ToImmutableArray);

            this.DeclaredConstructors = new[]
            {
                methodDesc.Object,
            };

            return this;
        }

        public InternalTypeDescriptionBuilder<T> WithDeclaredProperties(params IPropertyDescription[] props)
        {
            this.DeclaredProperties = props;
            return this;
        }

        public InternalTypeDescriptionBuilder<T> WithDeclaredProperties(params IFieldDescription[] fields)
        {
            this.DeclaredFields = fields;
            return this;
        }

        public T Build()
        {
            this._typeDescriptionMock
                .Setup(description => description.GetDeclaredConstructors())
                .Returns(this.DeclaredConstructors.ToImmutableArray());

            this._typeDescriptionMock
                .Setup(description => description.GetDeclaredFields())
                .Returns(this.DeclaredFields.ToImmutableArray());

            this._typeDescriptionMock
                .Setup(description => description.GetDeclaredProperties())
                .Returns(this.DeclaredProperties.ToImmutableArray());

            this._typeDescriptionMock
                .Setup(description => description.GetDeclaredMethods())
                .Returns(this.DeclaredMethod.ToImmutableArray);

            return this._typeDescriptionMock.Object;
        }

        #endregion
    }
}