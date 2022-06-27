using System;

namespace WebApi.BLL.Exceptions
{
    [System.Serializable]
    public class AuthentificationException : System.Exception
    {
        public AuthentificationException() { }
        public AuthentificationException(string message) : base(message) { }
        public AuthentificationException(string message, System.Exception inner) : base(message, inner) { }
        protected AuthentificationException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}