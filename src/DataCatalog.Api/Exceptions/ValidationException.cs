using System;
using System.Collections.Generic;
using System.Linq;

namespace DataCatalog.Api.Exceptions
{
    public class ValidationExceptionCollection : Exception
    {
        public ValidationExceptionCollection(string message, IEnumerable<ValidationException> exceptions) : base(GetMessage(message, exceptions)) { }

        static string GetMessage(string message, IEnumerable<ValidationException> exceptions) =>
            $"{message}\r\nEncountered the following validation errors:\r\n - {string.Join("\r\n - ", exceptions.Select(a => a.Message))}";
    }

    public class ValidationException : Exception
    {
        public ValidationException(string message, params string[] fields) : base(GetMessage(message, fields)) { }

        static string GetMessage(string message, params string[] fields) => 
            $"{message}\r\n   - Fields: {string.Join(", ", fields)}";
    }
}