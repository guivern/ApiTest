using System.Collections.Generic;
using System.Threading.Tasks;
using ApiTest.Data;
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

        [HttpGet]
        public async Task<IActionResult> List()
        {
            var ciudades = await _context.Ciudades.ToListAsync();

            // mapeamos los paises a su tipo dto
            var dto = _mapper.Map<List<CiudadListDto>>(ciudades);

            return Ok(dto);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Detail(long id)
        {
            // obtenemos la entidad desde la bd
            var ciudad = await _context.Ciudades
                .Include(c => c.Pais)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (ciudad == null) return NotFound();

            // mapeamos a su tipo dto
            var dto = _mapper.Map<CiudadDetailDto>(ciudad);

            return Ok(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CiudadDto dto)
        {
            // chequeamos si existe el pais asignado a la ciudad
            if (!await ExistePais(dto.IdPais))
                return BadRequest($"No existe un país con Id {dto.IdPais}");

            // mapeamos el dto a su tipo Entity
            var entity = _mapper.Map<Ciudad>(dto);

            // chequeamos si es capital
            if (dto.EsCapital) SetCapital(dto);

            // guardamos en la bd
            await _context.Ciudades.AddAsync(entity);
            await _context.SaveChangesAsync();

            // mapeamos a su tipo dto para retornar
            var dtoToReturn = _mapper.Map<CiudadDetailDto>(entity);

            return CreatedAtAction("Detail", new { Id = entity.Id }, dtoToReturn);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, CiudadDto dto)
        {
            // chequeamos que exista la entidad
            var entity = await _context.Ciudades.SingleOrDefaultAsync(c => c.Id == id);
            if (entity == null) return NotFound();

            // chequeamos si existe el pais asignado a la ciudad
            if (!await ExistePais(dto.IdPais))
                return BadRequest($"No existe un país con Id {dto.IdPais}");

            // chequeamos si es capital
            if (dto.EsCapital) SetCapital(dto);

            // mapeamos el dto a su tipo entidad
            entity = _mapper.Map<CiudadDto, Ciudad>(dto, entity);

            // actualizamos la entidad en la bd
            await _context.SaveChangesAsync();

            return NoContent();
        }

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
    }
}