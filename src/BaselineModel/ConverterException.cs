﻿using System;
using System.Runtime.Serialization;

namespace BaselineModel
{
    [Serializable]
    internal class ConverterException : Exception
    {
        public ConverterException()
        {
        }

        public ConverterException(string message) : base(message)
        {
        }

        public ConverterException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ConverterException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}