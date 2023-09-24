using APICatalogo.Context;
using APICatalogo.DTOs;
using APICatalogo.Models;
using APICatalogo.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace APICatalogo;


[Route("api/[controller]")]
[ApiController]
public class ProdutosController : ControllerBase
{
    private readonly IUnitOfWork _uof;
    private readonly IMapper _mapper;

    public ProdutosController(IUnitOfWork context, IMapper mapper)
    {
        _uof = context;
        _mapper = mapper;
    }

    [HttpGet("menorpreco")]
    public ActionResult<IEnumerable<ProdutoDTO>> GetProdutosPreco()
    {
        var produtos = _uof.ProdutoRepository.GetProdutosPorPreco().ToList();
        var produtosDTO = _mapper.Map<List<ProdutoDTO>>(produtos);

        return produtosDTO;
    }

    [HttpGet]
    public ActionResult<IEnumerable<ProdutoDTO>> Get()
    {
        var produtos = _uof.ProdutoRepository.Get().ToList();
        var produtosDTO = _mapper.Map<List<ProdutoDTO>>(produtos);

        return produtosDTO;
    }

    [HttpGet("{id}", Name="ObterProduto")]
    public ActionResult<ProdutoDTO> Get([FromQuery]int id)
    {
        var produto = _uof.ProdutoRepository.GetById(p => p.ProdutoId == id);

        if(produto is null)
        {
            return NotFound("Produto não encontrado...");
        }
        var produtoDTO = _mapper.Map<ProdutoDTO>(produto);

        return produtoDTO;
    }

    [HttpPost]
    public ActionResult Post([FromBody]ProdutoDTO produtoDto)
    {
        var produto = _mapper.Map<Produto>(produtoDto);

        _uof.ProdutoRepository.Add(produto);
        //persistrir no banco de dados
        _uof.Commit();

        var produtoDTO = _mapper.Map<ProdutoDTO>(produtoDto);

        return new CreatedAtRouteResult("ObterProduto",
            new { id = produto.ProdutoId }, produtoDTO);
    }

    [HttpPut("{id}")]
    public ActionResult Put(int id, [FromBody] ProdutoDTO produtoDto)
    {
        if (id != produtoDto.ProdutoId)
        {
            return BadRequest();
        }

        var produto = _mapper.Map<Produto>(produtoDto);
        _uof.ProdutoRepository.Update(produto);
        _uof.Commit();

        return Ok();
    }

    [HttpDelete("{id}")]
    public ActionResult <ProdutoDTO> Delete(int id)
    {
        var produto = _uof.ProdutoRepository.GetById(p => p.ProdutoId == id);
        //var produto = _uof.Produtos.Find(id);

        if(produto is null)
        {
            return NotFound("Produto não localizado...");
        }
        _uof.ProdutoRepository.Delete(produto);
        _uof.Commit();

        var produtoDto = _mapper.Map<ProdutoDTO>(produto);

        return produtoDto;
    }
}

