using APICatalogo.Context;
using APICatalogo.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace APICatalogo;


[Route("api/[controller]")]
[ApiController]
public class ProdutosController : ControllerBase
{
    private readonly AppDbContext _context;

    public ProdutosController(AppDbContext context)
    {
        _context = context;
    }


    [HttpGet("/primeiro")]
    public ActionResult<Produto> GetPrimeiro()
    {
        var produto = _context.Produtos.FirstOrDefault();
        if (produto is null)
        {
            return NotFound();
        }

        return produto;
    }

    [HttpGet]
    public ActionResult<IEnumerable<Produto>> Get()
    {
        var produtos = _context.Produtos.AsNoTracking().ToList();
        if (produtos is null)
        {
        //equivalente ao 404
        //bad request
        return NotFound("Produtos não encontrados...");
        }

        return produtos;
    }

    [HttpGet("{id}", Name="ObterProduto")]
    public async Task<ActionResult<Produto>> Get([FromQuery]int id)
    {
        var produto = await _context.Produtos.FirstOrDefaultAsync(p => p.ProdutoId == id);
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
        _context.Produtos.Add(produto);
        //persistrir no banco de dados
        _context.SaveChanges();

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
        _context.Entry(produto).State = EntityState.Modified;
        _context.SaveChanges();

        return Ok(produto);
    }

    [HttpDelete("{id}")]
    public ActionResult Delete(int id)
    {
        var produto = _context.Produtos.FirstOrDefault(p => p.ProdutoId == id);
        //var produto = _context.Produtos.Find(id);

        if(produto is null)
        {
            return NotFound("Produto não localizado...");
        }
        _context.Produtos.Remove(produto);
        _context.SaveChanges();

        return Ok(produto);
    }
}

