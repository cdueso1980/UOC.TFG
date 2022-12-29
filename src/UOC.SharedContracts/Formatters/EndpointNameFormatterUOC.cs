using System;
using MassTransit;

namespace UOC.SharedContracts
{
    public class EndpointNameFormatterUOC : IEndpointNameFormatter
    {
        public string Separator => throw new NotImplementedException();
        
        private IEndpointNameFormatter defaultFormatter;
        private string sagaPatternName;

        public EndpointNameFormatterUOC() 
        { 
            this.defaultFormatter = new DefaultEndpointNameFormatter(false);
        }

        public EndpointNameFormatterUOC(string sagaPatternName) : this()
        {
            this.sagaPatternName = sagaPatternName;
        }

        public string Message<T>() where T : class
        {
            return this.defaultFormatter.Message<T>();
        }

        public string Saga<T>() where T : class, ISaga
        {
            return $"{this.sagaPatternName}.{typeof(T).Name.Replace("Saga", string.Empty).ToLower()}";
        }

        public string Consumer<T>() where T : class, IConsumer
        {
            return this.defaultFormatter.Consumer<T>();
        }

        public string ExecuteActivity<T, TArguments>()
            where T : class, IExecuteActivity<TArguments>
            where TArguments : class
        {
            return this.defaultFormatter.ExecuteActivity<T, TArguments>();
        }

        public string CompensateActivity<T, TLog>()
            where T : class, ICompensateActivity<TLog>
            where TLog : class
        {
            return this.defaultFormatter.CompensateActivity<T, TLog>();
        }

        public string TemporaryEndpoint(string tag)
        {
            return this.defaultFormatter.TemporaryEndpoint(tag);
        }

        public string SanitizeName(string name)
        {
             return this.defaultFormatter.SanitizeName(name);
        }
    }
}
