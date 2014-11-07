using System;

namespace Data
{
    public class ItemNotFoundException<T> : Exception
    {
        public ItemNotFoundException(long key)
            : base("The '" + typeof(T).Name + "' item was not found for the key '" + key + "'.")
        {
        }

        public ItemNotFoundException()
            : base("No '" + typeof(T).Name + "' items were found.")
        {
        }
    }
}