using System;

namespace DataAccessLayer.Core
{
    public class TypeMismatchException : Exception
    {
        private readonly string _procedure;
        private readonly string _parameter;
        private readonly string _expected;
        private readonly string _actual;

        public TypeMismatchException(string procedure, string parameter, string expected, string actual)
        {
            _procedure = procedure;
            _parameter = parameter;
            _expected = expected;
            _actual = actual;
        }

        public override string Message
        {
            get
            {
                return string.Format(Messages.TypeMismatch, _procedure, _procedure, _expected, _actual);
            }
        }
    }
}
