namespace FoodSphere.SelfOrdering.Api.Services;

public class SelfOrderingAuthService(
    ILogger<SelfOrderingAuthService> logger,
    IOptions<EnvDomainApi> envDomainApi,
    IOptions<EnvDomainOrdering> envDomainOrdering
    // ConsumerService consumerService,
) {
    readonly EnvDomainApi envDomainApi = envDomainApi.Value;
    readonly EnvDomainOrdering envDomainOrdering = envDomainOrdering.Value;


}