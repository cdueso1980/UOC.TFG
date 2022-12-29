using System;
using MassTransit;

namespace UOC.SharedContracts
{
    public class MessageNameFormatterUOC<T> : IMessageEntityNameFormatter<T> where T : class
    {
        public string FormatEntityName()
        {
            var currentType = typeof(T);
            if(!currentType.IsGenericType)
                 return $"Message.{typeof(T).Name}";

            return $"Message.{typeof(T).Name.Split('`')[0]}<{currentType.GetGenericArguments()[0].Name}>";
        }
    }
}


