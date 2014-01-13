using System;

namespace DataAccessLayer.Core
{
    public class CapacityException : Exception
    {
        private readonly int _capacity;
        
        public CapacityException(int capacity)
        {
            _capacity = capacity;
        }

        public override string Message
        {
            get
            {
                return string.Format(Messages.Capacity, _capacity);
            }
        }
    }
}
