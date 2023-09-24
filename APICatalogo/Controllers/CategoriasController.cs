using APICatalogo.Context;
using APICatalogo.Models;
using AutoMapper;
using APICatalogo.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using APICatalogo.DTOs;

namespace APICatalogo.Controllers;


[Route("api/[controller]")]
[ApiController]
public class CategoriasController : ControllerBase
{
    private readonly IUnitOfWork _uof;
    private readonly IMapper _mapper;

    public CategoriasController(IUnitOfWork context, IMapper mapper)
    {
        _uof = context;
        _mapper = mapper;
    }

    [HttpGet("produtos")]
    public ActionResult<IEnumerable<CategoriaDTO>> GetCategoriasProdutos()
    {
        // método include carrega as entidades relacionadas
        // (no caso Produtos as categorias)
        // return _context.Categorias.Include(p => p.Produtos).ToList();
        //utilizando filtro, boa prática.
        var categorias = _uof.CategoriaRepository.GetCategoriasProdutos().ToList();
        var cetagoriasDTO = _mapper.Map<List<CategoriaDTO>>(categorias);

        return cetagoriasDTO;
    }

    [HttpGet]
    public ActionResult<IEnumerable<CategoriaDTO>> Get()
    {
        try
        {
            var categorias = _uof.ProdutoRepository.Get().ToList();
            var categoriaDTO = _mapper.Map<List<CategoriaDTO>>(categorias);

            return categoriaDTO;

        } catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                "Ocorreu um problema ao tratar a sua solicitação");
        }
    }

    [HttpGet("{id:int}", Name = "ObterCategoria")]
    public ActionResult<CategoriaDTO> Get([FromQuery] int id)
    {
        try
        {
            var categoria = _uof.CategoriaRepository.GetById(p => p.CategoriaId == id);

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

    [HttpPost]
    public ActionResult Post([FromBody]CategoriaDTO categoriaDto)
    {
        var categoria = _mapper.Map<Categoria>(categoriaDto);

        _uof.CategoriaRepository.Add(categoria);
        _uof.Commit();

        var categoriaDTO = _mapper.Map<CategoriaDTO>(categoriaDto);

        return new CreatedAtRouteResult("ObterCategoria",
                new { id = categoria.CategoriaId }, categoriaDTO);
    }

    [HttpPut("{id}")]
    public ActionResult Put(int id, [FromBody] CategoriaDTO categoriaDto)
    {
        if(id != categoriaDto.CategoriaId)
        {
            return BadRequest();
        }

        var categoria = _mapper.Map<Categoria>(categoriaDto);
        _uof.CategoriaRepository.Update(categoria);
        _uof.Commit();
        return Ok();
    }

    [HttpDelete("{id}")]
    public ActionResult<CategoriaDTO> Delete(int id)
    {
        var categoria = _uof.CategoriaRepository.GetById(p => p.CategoriaId == id);

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
