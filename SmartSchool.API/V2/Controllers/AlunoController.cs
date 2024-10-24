using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SmartSchool.API.Repository;
using SmartSchool.Helpers;
using SmartSchool.Models;
using SmartSchool.V1.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartSchool.API.V2.Controllers
{
    /// <summary>
    /// Version 2.0 - Controller de Aluno.
    /// </summary>
    /// 
    [ApiController]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class AlunoController : ControllerBase
    { 
        private readonly IBaseRepository _repository;
        public readonly IMapper _mapper;

        public AlunoController(IBaseRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] PageParams pageParams) 
        {
            var alunos = await _repository.GetAllAlunosAsync(pageParams, true);

            var alunosResult = _mapper.Map<IEnumerable<AlunoDto>>(alunos);

            Response.AddPagination(alunos.CurrentPage, alunos.PageSize, alunos.TotalCount, alunos.TotalPages);

            return Ok(alunosResult);
        }

        [HttpPost]
        public IActionResult Post(AlunoRegistrarDto model)
        {
            var aluno = _mapper.Map<Aluno>(model);

            _repository.Add(aluno);

            if (_repository.SaveChanges())
            {
                return Created($"/api/aluno/{model.Id}", _mapper.Map<AlunoDto>(aluno));
            }

            return BadRequest("Aluno não cadastrado");
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, AlunoRegistrarDto model)
        {
            var aluno = _repository.GetAlunoById(id, false);

            if (aluno == null)
                return BadRequest("Aluno não encontrado");

            _mapper.Map(model, aluno);

            _repository.Update(aluno);

            if (_repository.SaveChanges())
            {
                return Created($"/api/aluno/{model.Id}", _mapper.Map<AlunoDto>(aluno));
            }

            return BadRequest("Aluno não atualizado");
        }

        [HttpPatch("{id}")]
        public IActionResult Patch(int id, AlunoRegistrarDto model)
        {
            var aluno = _repository.GetAlunoById(id, false);

            if (aluno == null) 
                return BadRequest("Aluno não encontrado");

            _mapper.Map(model, aluno);

            _repository.Update(aluno);

            if (_repository.SaveChanges())
            {
                return Created($"/api/aluno/{model.Id}", _mapper.Map<AlunoDto>(aluno)); // mapeando Aluno para model AlunoDto
            }

            return BadRequest("Aluno não atualizado");
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var aluno = _repository.GetAlunoById(id, false);

            if (aluno == null) 
                return BadRequest("Aluno não encontrado");

            _repository.Delete(aluno);

            if (_repository.SaveChanges())
            {
                return Ok("Aluno deletado");
            }

            return BadRequest("Aluno não foi deletado");
        }
    }
}
