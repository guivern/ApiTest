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
    public class CiudadesController : ControllerBase
    {
        private DataContext _context;
        private IMapper _mapper;

        public CiudadesController(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        /// Obtiene lista de ciudades.
        /// </summary>
        /// <param name="filter">Filtro a ser aplicado. Ej: Asuncion</param>
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
            var query = _context.Ciudades.Include(c => c.Pais).AsQueryable();

            query = Filter(query, filter);
            query = OrderBy(query, orderBy);

            var ciudades = await PagedList<Ciudad>.CreateAsync(query, pageNumber, pageSize);

            // mapeamos los paises a su tipo dto
            var dto = _mapper.Map<List<CiudadListDto>>(ciudades);

            // header de paginacion
            Response.AddPagination(ciudades.PageNumber, ciudades.PageSize, ciudades.TotalPages, ciudades.TotalCount);

            return Ok(dto);
        }

        /// <summary>
        /// Obtiene los detalles de una ciudad específica.
        /// </summary>
        /// <param name="id">Id. de la ciudad a consultar</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> Detail(long id)
        {
            var ciudad = await _context.Ciudades
                .Include(c => c.Pais)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (ciudad == null) return NotFound();

            // mapeamos a su tipo dto
            var dto = _mapper.Map<CiudadDetailDto>(ciudad);

            return Ok(dto);
        }

        /// <summary>
        /// Crea una ciudad.
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Create(CiudadDto dto)
        {
            // chequeamos si existe el pais asignado a la ciudad
            if (!await ExistePais((long)dto.IdPais))
                return BadRequest($"No existe un país con Id {dto.IdPais}");

            // mapeamos el dto al tipo Ciudad
            var entity = _mapper.Map<Ciudad>(dto);

            // chequeamos si es capital
            if (dto.EsCapital) SetCapital(dto);

            // guardamos
            await _context.Ciudades.AddAsync(entity);
            await _context.SaveChangesAsync();

            // mapeamos a su tipo dto para retornar
            var dtoToReturn = _mapper.Map<CiudadDetailDto>(entity);

            return CreatedAtAction("Detail", new { Id = entity.Id }, dtoToReturn);
        }

        /// <summary>
        /// Actualiza una ciudad específica.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, CiudadDto dto)
        {
            // chequeamos si existe la Ciudad
            var ciudad = await _context.Ciudades.SingleOrDefaultAsync(c => c.Id == id);
            if (ciudad == null) return NotFound();

            // chequeamos si existe el pais asignado a la ciudad
            if (!await ExistePais((long)dto.IdPais))
                return BadRequest($"No existe un país con Id {dto.IdPais}");

            // chequeamos si es capital
            if (dto.EsCapital) SetCapital(dto);

            // mapeamos el dto al tipo Ciudad
            ciudad = _mapper.Map<CiudadDto, Ciudad>(dto, ciudad);

            // actualizamos
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Elimina una ciudad específica.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            // obtenemos el pais desde la bd
            var ciudad = await _context.Ciudades
                .FirstOrDefaultAsync(c => c.Id == id);

            // chequeamos que exista el pais
            if (ciudad == null) return NotFound();

            _context.Ciudades.Remove(ciudad);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        #region metodos privados
        // como un pais solo puede tener una capital, si la ciudad a crear o modificar es capital
        // entonces debemos setear la propiedad esCapital en false para las otras ciudades del pais
        // y con esto evitar la posibilidad de que un pais tenga mas de una capital.
        private async void SetCapital(CiudadDto dto)
        {
            var pais = await _context.Paises
                    .Include(p => p.Ciudades)
                    .FirstOrDefaultAsync(p => p.Id == dto.IdPais);

            foreach (var ciudad in pais.Ciudades)
            {
                ciudad.EsCapital = false;
            }
        }

        private async Task<bool> ExistePais(long idPais)
        {
            return await _context.Paises.AnyAsync(p => p.Id == idPais);
        }

        private IQueryable<Ciudad> Filter(IQueryable<Ciudad> query, string value)
        {
            if (string.IsNullOrEmpty(value)) return query;

            return query.Where(c => c.Nombre.ToLower().Contains(value.ToLower())
                || c.EsCapital.ToString().ToLower().Equals(value.ToLower())
                || c.Pais.Nombre.ToLower().Contains(value.ToLower()));
        }

        private IQueryable<Ciudad> OrderBy(IQueryable<Ciudad> query, string orderBy)
        {
            if (string.IsNullOrEmpty(orderBy)) return query;

            var splitedOrder = orderBy.Split(':');
            var columnName = splitedOrder[0].ToLower();
            var orderType = splitedOrder.Count() > 1 ? splitedOrder[1] : "asc";

            if (orderType.ToLower().Equals("desc"))
            {
                return query.OrderByDescending(OrderByExpressions.CiudadesOrderBy[columnName]);
            }

            return query.OrderBy(OrderByExpressions.CiudadesOrderBy[columnName]);
        }
        #endregion
    }
}