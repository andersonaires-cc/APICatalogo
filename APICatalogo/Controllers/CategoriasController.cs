using APICatalogo.Context;
using APICatalogo.Models;
using AutoMapper;
using APICatalogo.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using APICatalogo.DTOs;
using APICatalogo.Pagination;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Cors;

namespace APICatalogo.Controllers;

[Produces("application/json")]
[Route("api/[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[EnableCors("PermitirApiRequest")]
public class CategoriasController : ControllerBase
{
    private readonly IUnitOfWork _uof;
    private readonly IMapper _mapper;

    public CategoriasController(IUnitOfWork context, IMapper mapper)
    {
        _uof = context;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoriaDTO>>> Get([FromQuery] CategoriasParameters categoriasParameters)
    {

        var categorias = await _uof.CategoriaRepository.GetCategorias(categoriasParameters);

        var metadata = new
        {
            categorias.TotalCount,
            categorias.PageSize,
            categorias.CurrentPage,
            categorias.TotalPages,
            categorias.HasNext,
            categorias.HasPrevious
        };

        Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

        var cetagoriasDTO = _mapper.Map<List<CategoriaDTO>>(categorias);

        return cetagoriasDTO;
    }

    //[HttpGet]
    //public ActionResult<IEnumerable<CategoriaDTO>> Get()
    //{
    //    try
    //    {
    //        var categorias = _uof.CategoriaRepository.Get().ToList();
    //        var categoriaDTO = _mapper.Map<List<CategoriaDTO>>(categorias);

    //        return categoriaDTO;

    //    } catch (Exception)
    //    {
    //        return StatusCode(StatusCodes.Status500InternalServerError,
    //            "Ocorreu um problema ao tratar a sua solicitação");
    //    }
    //}

    /// <summary>
    /// Obtem uma categoria pelo seu Id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id:int}", Name = "ObterCategoria")]
    public async Task<ActionResult<CategoriaDTO>> Get(int id)
    {
        try
        {
            var categoria = await _uof.CategoriaRepository.GetById(p => p.CategoriaId == id);

            if (categoria == null)
            {
                return NotFound($"Categoria com id= {id} não encontrada...");
            }

            var categoriaDTO = _mapper.Map<CategoriaDTO>(categoria);

            return categoriaDTO;
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
               "Ocorreu um problema ao tratar a sua solicitação");
        }

    }

    /// <summary>
    /// Inclui uma nova Categoria
    /// </summary>
    /// <remarks>
    /// Exemplo de request:
    /// 
    ///     POST api/categorias
    ///     {
    ///         "categoriaId" : 1,
    ///         "nome" : "categoria1",
    ///         "imagemUrl" : "http//teste.net/1.jpg"
    ///      }
    /// </remarks>
    /// <param name="categoriaDto">objeto categoria</param>
    /// <remarks>Retorna um onbjeto Categoria incluida</remarks>
    [HttpPost]
    public async Task<ActionResult> Post(CategoriaDTO categoriaDto)
    {
        var categoria = _mapper.Map<Categoria>(categoriaDto);

        _uof.CategoriaRepository.Add(categoria);
        await _uof.Commit();

        var categoriaDTO = _mapper.Map<CategoriaDTO>(categoria);

        return new CreatedAtRouteResult("ObterCategoria",
                new { id = categoria.CategoriaId }, categoriaDTO);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Put(int id, [FromBody] CategoriaDTO categoriaDto)
    {
        if(id != categoriaDto.CategoriaId)
        {
            return BadRequest();
        }

        var categoria = _mapper.Map<Categoria>(categoriaDto);
        _uof.CategoriaRepository.Update(categoria);
        await _uof.Commit();
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<CategoriaDTO>> Delete(int id)
    {
        var categoria = await _uof.CategoriaRepository.GetById(p => p.CategoriaId == id);

        if(categoria == null)
        {
            return NotFound("Categoria não encontrada");
        }
        _uof.CategoriaRepository.Delete(categoria);
        _uof.Commit();
        var produtoDto = _mapper.Map<CategoriaDTO>(categoria);

        return produtoDto;
    }
}
