using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SourceGeneratorConsole
{
    [AttributeUsage(AttributeTargets.Class, 
        Inherited = true, 
        AllowMultiple = false)]
    public class DecoratorGeneratorAttribute : Attribute
    {

    }
}
