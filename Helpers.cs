using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http.HttpResults;

namespace OrleansEmailApp;

public static class Helpers
{
    public static string ExtractEmailDomain(string email)
    {
        if (email is null or "")
            throw new BadHttpRequestException("This is not a valid email");

        string[] splitEmail = email.Split("@");
        
        if (splitEmail.Length != 2)
            throw new BadHttpRequestException("This is not a valid email");

        return splitEmail[1];
    }
}