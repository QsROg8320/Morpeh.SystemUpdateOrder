using System;

namespace Morpeh.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class UpdateAfterAttribute : Attribute
    {
        public Type Type { get; private set; }
        public UpdateAfterAttribute(Type type) { Type = type; }
    }

}
