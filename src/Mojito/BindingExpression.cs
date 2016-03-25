using System;

namespace Mojito
{
    public interface IBindingExpression<in T>
    {
        void To<T2>() where T2 : T;
    }

    public class BindingExpression<T> : IBindingExpression<T>
    {
        public BindingExpression(IMojitoContainer mojitoContainer)
        {
            _mojitoContainer = mojitoContainer;
        }

        public void To<T2>() where T2 : T
        {
            _mojitoContainer.Register<T>(() => Activator.CreateInstance<T2>());
        }

        private readonly IMojitoContainer _mojitoContainer;
    }
}