using APICatalogo.Context;
using APICatalogo.Models;
using APICatalogo.Repository;
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

    public ProdutosController(IUnitOfWork context)
    {
        _uof = context;
    }

    [HttpGet("menorpreco")]
    public ActionResult<IEnumerable<Produto>> GetProdutosPreco()
    {
        return _uof.ProdutoRepository.GetProdutosPorPreco().ToList();
    }

    [HttpGet]
    public ActionResult<IEnumerable<Produto>> Get()
    {
        var produtos = _uof.ProdutoRepository.Get().ToList();
        if (produtos is null)
        {
        //equivalente ao 404
        //bad request
        return NotFound("Produtos não encontrados...");
        }

        return produtos;
    }

    [HttpGet("{id}", Name="ObterProduto")]
    public ActionResult<Produto> Get([FromQuery]int id)
    {
        var produto = _uof.ProdutoRepository.GetById(p => p.ProdutoId == id);

        if(produto is null)
        {
            return NotFound("Produto não encontrado...");
        }
        return produto;
    }

    [HttpPost]
    public ActionResult Post(Produto produto)
    {
        if(produto is null)
        {
            return BadRequest();
        }
        _uof.ProdutoRepository.Add(produto);
        //persistrir no banco de dados
        _uof.Commit();

        return new CreatedAtRouteResult("ObterProduto",
            new { id = produto.ProdutoId }, produto);
    }

    [HttpPut("{id}")]
    public ActionResult Put(int id, Produto produto)
    {
        if (id != produto.ProdutoId)
        {
            return BadRequest();
        }
        _uof.ProdutoRepository.Update(produto);
        _uof.Commit();

        return Ok(produto);
    }

    [HttpDelete("{id}")]
    public ActionResult Delete(int id)
    {
        var produto = _uof.ProdutoRepository.GetById(p => p.ProdutoId == id);
        //var produto = _uof.Produtos.Find(id);

        if(produto is null)
        {
            return NotFound("Produto não localizado...");
        }
        _uof.ProdutoRepository.Delete(produto);
        _uof.Commit();

        return Ok(produto);
    }
}

