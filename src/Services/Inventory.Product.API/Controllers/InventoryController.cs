using Inventory.Product.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs.Inventory;
using Shared.SeedWork;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace Inventory.Product.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _service;

        public InventoryController(IInventoryService service)
        {
            _service = service;
        }

        /// <summary>
        /// api/inventory/item/{itemNo}
        /// </summary>
        /// <param name="itemNo"></param>
        /// <returns></returns>
        [Route("items/{itemNo}", Name = "GetAllByItemNo")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<InventoryEntryDto>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<InventoryEntryDto>>> GetAllByItemNo([Required] string itemNo)
        {
            var result = await _service.GetAllByItemNoAsync(itemNo);
            return Ok(result);
        }

        /// <summary>
        /// api/inventory/item/{itemNo}/paging
        /// </summary>
        /// <param name="itemNo"></param>
        /// <returns></returns>
        [Route("items/{itemNo}/paging", Name = "GetAllByItemNoPaging")]
        [HttpGet]
        [ProducesResponseType(typeof(PagedList<InventoryEntryDto>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<PagedList<InventoryEntryDto>>> GetAllByItemNoPaging([Required] string itemNo,
            [FromQuery] GetInventoryPagingQuery query)
        {
            query.SetItemNo(itemNo);
            var result = await _service.GetAllByItemNoPagingAsync(query);
            return Ok(result);
        }

        /// <summary>
        /// api/inventory/puchase/{itemNo}
        /// </summary>
        /// <param name="itemNo"></param>
        /// <returns></returns>
        [HttpPost("purchase/{itemNo}", Name = "PurchaseOrder")]
        [ProducesResponseType(typeof(InventoryEntryDto), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<InventoryEntryDto>> PurchaseOrder(
            [Required] string itemNo,
            [FromBody] PurchaseProductDto model)
        {
            var result = await _service.PurchaseItemAsync(itemNo, model);
            return Ok(result);
        }

        /// <summary>
        /// api/inventory/{id}
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id}", Name = "DeleteById")]
        public async Task<IActionResult> DeleteById(
            [Required] string id)
        {
            var entity = await _service.GetByIdAsync(id);
            if (entity == null) return NotFound();

            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
}
