using System;

namespace Scellecs.Morpeh.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class UpdateAfterAttribute : Attribute
    {
        public Type Type { get; private set; }
        public UpdateAfterAttribute(Type type) { Type = type; }
    }
}
