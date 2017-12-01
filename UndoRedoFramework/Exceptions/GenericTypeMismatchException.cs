// From http://wpfundoredo.codeplex.com/

using System;
using System.Runtime.Serialization;

namespace InfinityGroup.VesselMonitoring.UndoRedoFramework.Exceptions
{
    [DataContract]
    public class GenericTypeMismatchException : Exception
    {

        public GenericTypeMismatchException(Type expectedType, Type actualType) :
            base(string.Format
            ("This class implements an interface which\ntakes an object as a parameter.\nHowever, the class is generically typed,\nand will only accept objects matching the type\nwith which it was initialized.\n\nSummary\nExpected:{0} Recieved:{1}\n\nDetailed\nExpected:{2} Recieved:{3}", expectedType.Name, actualType.Name, expectedType.FullName, actualType.FullName))
        { }
    }
}
