using System;
using Microsoft.Rest;

namespace Eryph.ClientRuntime.Authentication
{
    public class AuthenticationException : RestException
    {

        /// <summary>
        /// Initializes a new instance of the AuthenticationException class.
        /// </summary>
        public AuthenticationException()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the AuthenticationException class.
        /// </summary>
        /// <param name="message">Exception message.</param>
        public AuthenticationException(string message)
            : this(message, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the AuthenticationException class.
        /// </summary>
        /// <param name="message">Exception message.</param>
        /// <param name="innerException">Inner exception.</param>
        public AuthenticationException(string message, Exception innerException) 
            : base(message, innerException)
        {
        }
        
    }
}