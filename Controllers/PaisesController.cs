using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiTest.Data;
using ApiTest.Helpers;
using ApiTest.Models;
using ApiTest.Models.Dtos;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiTest.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class PaisesController : ControllerBase
    {
        private DataContext _context;
        private IMapper _mapper;

        public PaisesController(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        /// Obtiene lista de paises.
        /// </summary>
        /// <param name="filter">Filtro a ser aplicado. Ej: Paraguay</param>
        /// <param name="orderBy">Ordenamiento a ser aplicado (asc | desc). Ej: nombre:desc</param>
        /// <param name="pageSize">Tamaño de página</param>
        /// <param name="pageNumber">Nro. de página</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> List(
            [FromQuery] string filter,
            [FromQuery] string orderBy = Constants.DEFAULT_ODERING,
            [FromQuery] int pageSize = Constants.DEFAULT_PAGE_SIZE,
            [FromQuery] int pageNumber = Constants.DEFAULT_PAGE_NUMBER)
        {
            var query = _context.Paises.Include(p => p.Ciudades).AsQueryable();

            query = Filter(query, filter);
            query = OrderBy(query, orderBy);

            var paises = await PagedList<Pais>.CreateAsync(query, pageNumber, pageSize);

            // mapeamos los paises a su tipo dto
            var dto = _mapper.Map<List<PaisListDto>>(paises);

            // header de paginacion
            Response.AddPagination(paises.PageNumber, paises.PageSize, paises.TotalPages, paises.TotalCount);

            return Ok(dto);
        }

        /// <summary>
        /// Obtiene los detalles de un país específico.
        /// </summary>
        /// <param name="id">Id. del país a consultar</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> Detail(long id)
        {
            var pais = await _context.Paises
                .Include(p => p.Ciudades)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pais == null) return NotFound();

            // mapeamos a su tipo dto
            var dto = _mapper.Map<PaisDetailDto>(pais);

            return Ok(dto);
        }

        /// <summary>
        /// Crea un país.
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Create(PaisDto dto)
        {
            // mapeamos el dto al tipo Pais
            var pais = _mapper.Map<Pais>(dto);

            // guardamos
            await _context.Paises.AddAsync(pais);
            await _context.SaveChangesAsync();

            // mapeamos a su tipo dto para retornar
            var dtoToReturn = _mapper.Map<PaisDetailDto>(pais);

            return CreatedAtAction("Detail", new { Id = pais.Id }, dtoToReturn);
        }

        /// <summary>
        /// Actualiza un país específico.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, PaisDto dto)
        {
            // chequeamos si existe el Pais
            var pais = await _context.Paises.SingleOrDefaultAsync(p => p.Id == id);
            if (pais == null) return NotFound();

            // mapeamos el dto al tipo Pais
            pais = _mapper.Map<PaisDto, Pais>(dto, pais);

            // actualizamos
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Elimina un país específico.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var pais = await _context.Paises
                .Include(p => p.Ciudades)
                .FirstOrDefaultAsync(p => p.Id == id);

            // chequeamos que exista el pais
            if (pais == null) return NotFound();

            // chequeamos que no tenga ciudades asociadas
            if (pais.Ciudades.Count > 0)
                return BadRequest("No se puede eliminar porque tiene ciudades asociadas");

            _context.Paises.Remove(pais);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        #region metodos privados
        private IQueryable<Pais> Filter(IQueryable<Pais> query, string value)
        {
            if (string.IsNullOrEmpty(value)) return query;

            return query.Where(p => p.Nombre.ToLower().Contains(value.ToLower())
                || p.Sigla.ToLower().Contains(value.ToLower()));
        }

        private IQueryable<Pais> OrderBy(IQueryable<Pais> query, string orderBy)
        {
            if (string.IsNullOrEmpty(orderBy)) return query;

            var splitedOrder = orderBy.Split(':');
            var columnName = splitedOrder[0].ToLower();
            var orderType = splitedOrder.Count() > 1 ? splitedOrder[1] : "asc";

            if (orderType.ToLower().Equals("desc"))
            {
                return query.OrderByDescending(OrderByExpressions.PaisesOrderBy[columnName]);
            }

            return query.OrderBy(OrderByExpressions.PaisesOrderBy[columnName]);
        }
        #endregion
    }
}