using FoodSphere.Infrastructure.Repository;
using MassTransit;

namespace FoodSphere.Pos.Api.Service;

// public class BillService(
//     IPublishEndpoint publishEndpoint,
//     PersistenceService persistenceService,
//     BillRepository billRepository,
//     TableRepository tableService,
//     MenuRepository menuRepository
// ) : BillServiceBase(
//     publishEndpoint,
//     persistenceService,
//     billRepository,
//     menuRepository,
//     tableService)
// {
//     // เช็คบิลว่าตรงกับของ restaurant ที่ path ไหม ผ่าน auth
// }