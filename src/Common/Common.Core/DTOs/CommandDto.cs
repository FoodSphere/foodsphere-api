namespace FoodSphere.Common.DTO;

public record BillCreateCommand
(
    TableKey TableKey,
    ConsumerUserKey? ConsumerKey,
    short? Pax
);

public record BillUpdateCommand
(
    short TableId,
    Guid? ConsumerId,
    short? Pax
);

public record PortalUpdateCommand
(
    short? MaxUsage,
    TimeSpan? ValidDuration
);

public record OrderingPortalCreateCommand
(
    BillKey BillKey,
    short? MaxUsage,
    TimeSpan? ValidDuration
);

public record OrderCreateCommand
(
    IEnumerable<OrderItemCreateCommand> Items,
    OrderStatus Status = OrderStatus.Pending
);

public record OrderItemCreateCommand
(
    MenuKey MenuKey,
    short Quantity,
    string? Note
);

public record OrderItemUpdateCommand
(
    short Quantity,
    string? Note
);

public record IngredientCreateCommand
(
    RestaurantKey RestaurantKey,
    string Name,
    IEnumerable<TagKey> Tags,
    string? Unit,
    string? Description,
    IngredientStatus Status
);

public record IngredientUpdateCommand
(
    string Name,
    IEnumerable<TagKey> Tags,
    string? Unit,
    string? Description,
    IngredientStatus Status
);

public record MenuCreateCommand
(
    RestaurantKey RestaurantKey,
    string Name,
    int Price,
    IEnumerable<TagKey> Tags,
    IEnumerable<(IngredientKey, decimal)> Ingredients,
    IEnumerable<(MenuKey, short)> Components,
    string? DisplayName,
    string? Description,
    MenuStatus Status
);

public record MenuUpdateCommand
(
    string Name,
    int Price,
    IEnumerable<TagKey> Tags,
    IEnumerable<(IngredientKey, decimal)> Ingredients,
    IEnumerable<(MenuKey, short)> Components,
    string? DisplayName,
    string? Description,
    MenuStatus Status
);

public record StockCreateCommand
(
    BranchKey BranchKey,
    IngredientKey IngredientKey,
    decimal Amount,
    string? Note
);

public record RestaurantCreateCommand
(
    MasterUserKey OwnerKey,
    string Name,
    string? DisplayName,
    Contact? Contact
);

public record RestaurantUpdateCommand
(
    string Name,
    string? DisplayName,
    Contact? Contact
);

public record RestaurantStaffCreateCommand
(
    RestaurantKey RestaurantKey,
    MasterUserKey MasterKey,
    string DisplayName,
    IEnumerable<RoleKey> RoleKeys
);

public record RestaurantStaffUpdateCommand
(
    string DisplayName
);

public record BranchStaffCreateCommand
(
    BranchKey BranchKey,
    MasterUserKey MasterKey,
    string DisplayName,
    IEnumerable<RoleKey> RoleKeys
);

public record BranchStaffUpdateCommand
(
    string DisplayName
);

public record BranchCreateCommand
(
    RestaurantKey RestaurantKey,
    string Name,
    string? DisplayName,
    string? Address,
    TimeOnly? OpeningTime,
    TimeOnly? ClosingTime,
    Contact? Contact
);

public record BranchUpdateCommand
(
    string Name,
    string? DisplayName,
    string? Address,
    TimeOnly? OpeningTime,
    TimeOnly? ClosingTime,
    Contact? Contact
);

public record RoleCreateCommand
(
    RestaurantKey RestaurantKey,
    string Name,
    IEnumerable<PermissionKey> PermissionKeys,
    string? Description
);

public record RoleUpdateCommand
(
    string Name,
    // IEnumerable<PermissionKey> PermissionKeys,
    string? Description
);

public record WorkerCreateCommand
(
    BranchKey BranchKey,
    string Name,
    IEnumerable<RoleKey> RoleKeys,
    string? Phone
);

public record WorkerUpdateCommand
(
    string Name,
    IEnumerable<RoleKey> RoleKeys,
    string? Phone
);

public record WorkerPortalCreateCommand
(
    WorkerUserKey WorkerKey,
    TimeSpan? ValidDuration
);

public record TableCreateCommand
(
    BranchKey BranchKey,
    string? Name
);

public record TableUpdateCommand
(
    string? Name
);

public record TagCreateCommand
(
    RestaurantKey RestaurantKey,
    string Name,
    string? Type
);

public record TagUpdateCommand
(
    string Name
);

public record MemberCreateCommand
(
    BillKey BillKey,
    ConsumerUserKey? ConsumerKey,
    string? Name
);

public record MemberUpdateCommand
(
    string? Name
);

public record ConsumerCreateCommand
(
    string Email,
    string Password,
    string Username,
    string? PhoneNumber,
    bool TwoFactorEnabled = false
);

public record CashPaymentCreateCommand
(
    BillKey BillKey
);

public record StripePaymentCreateCommand
(
    BillKey BillKey,
    string SessionId,
    decimal Amount
);

public record ServiceRequestCreateCommand
(
    BillKey BillKey,
    ConsumerUserKey? ConsumerKey,
    string Reason
);

public record ServiceRequestStatusCommand
(
    ServiceRequestStatus Status
);