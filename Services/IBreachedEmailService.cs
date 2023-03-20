namespace OrleansEmailApp.Services;

public interface IBreachedEmailService
{
    Task<string?> AddEmailToBreachedList(string email);
    Task<string?> CheckIfEmailIsBreached(string email);
}