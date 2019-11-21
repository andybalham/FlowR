using System;

namespace BusinessExample.Core.Exceptions
{
    public class ResourceException : Exception
    {
    }

    public class NotFoundResourceException : ResourceException
    {
    }

    public class UnauthorizedResourceException : ResourceException
    {
    }
}
