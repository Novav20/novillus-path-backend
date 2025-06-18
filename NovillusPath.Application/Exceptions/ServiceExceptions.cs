namespace NovillusPath.Application.Exceptions;

// Define a custom exception for authorization errors within the service
public class ServiceAuthorizationException(string message) : Exception(message)
{
}

// Define a custom exception for not found errors within the service
public class ServiceNotFoundException(string message) : Exception(message)
{
}

// Define a custom exception for bad request errors within the service
public class ServiceBadRequestException(string message) : Exception(message)
{
}
