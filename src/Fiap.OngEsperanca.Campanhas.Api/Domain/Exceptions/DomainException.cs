using System;

namespace Fiap.OngEsperanca.Campanhas.Api.Domain.Exceptions;

public class DomainException : Exception
{
    public DomainException(string message) : base(message)
    {
    }
}