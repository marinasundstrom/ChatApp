using MassTransit;

namespace ChatApp.Features.Chat;

public static class MassTransitExtensions
{
    public static IBusRegistrationConfigurator AddTodoConsumers(this IBusRegistrationConfigurator busRegistrationConfigurator)
    {
        //busRegistrationConfigurator.AddConsumer<UpdateStatusConsumer>();

        return busRegistrationConfigurator;
    }
}
