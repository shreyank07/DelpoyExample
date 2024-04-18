using ProdigyFramework.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyFramework.ComponentModel
{
    /// <summary>
    /// Generates AppException customized exceptions
    /// </summary>
    public sealed class AppException : Exception
    {
        readonly string m_string;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppException" /> class.
        /// </summary>
        public AppException()
        {
            this.ExceptionLevel = TypeOfMessage.LOG;
            this.m_string = "";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppException" /> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public AppException(string message) : base(message)
        {
            this.ExceptionLevel = TypeOfMessage.LOG;
            this.m_string = message;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppException" /> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        public AppException(string message, Exception inner) : base(message, inner)
        {
            this.ExceptionLevel = TypeOfMessage.LOG;
            this.m_string = message;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppException" /> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="level">The level.</param>
        public AppException(string message, TypeOfMessage level) : base(message)
        {
            this.ExceptionLevel = level;
            this.m_string = message;
        }

        /// <summary>
        /// Gets or sets the exception level.
        /// </summary>
        /// <value>The exception level.</value>
        public TypeOfMessage ExceptionLevel { get; set; }
    }
}
