using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SmartSchool.API.Repository;
using SmartSchool.Helpers;
using SmartSchool.Models;
using SmartSchool.V1.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartSchool.API.V1.Controllers
{
    /// <summary>
    /// Version 1.0 - Controller de Aluno.
    /// </summary>
    /// 
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class AlunoController : ControllerBase
    {
        private readonly IBaseRepository _repository;
        public readonly IMapper _mapper;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="mapper"></param>
        public AlunoController(IBaseRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        /// <summary>
        /// Método responsavel por retornar apenas um aluno
        /// </summary>
        /// <returns></returns>
        // api/V1/Aluno/GetRegister
        [HttpGet("GetRegister")]
        public IActionResult GetRegister()
        {
            return Ok(new AlunoRegistrarDto());
        }

        /// <summary>
        /// Método responsável para retornar todos os alunos
        /// </summary>
        /// <returns></returns>
        // GET: api/V1/Aluno
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] PageParams pageParams)
        {
            var alunos = await _repository.GetAllAlunosAsync(pageParams, true);

            var alunosResult = _mapper.Map<IEnumerable<AlunoDto>>(alunos);

            Response.AddPagination(alunos.CurrentPage, alunos.PageSize, alunos.TotalCount, alunos.TotalPages);

            return Ok(alunosResult);
        }

        /// <summary>
        /// Método responsável por retornar apenas um aluno por meio do Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: api/V1/Aluno/5
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var aluno = _repository.GetAlunoById(id, false);
            if (aluno == null) return BadRequest("O Aluno não foi encontrado");

            // Dto
            var alunoDto = _mapper.Map<AlunoDto>(aluno);

            return Ok(alunoDto);
        }

        /// <summary>
        /// Método responsável por criar aluno 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        // POST: api/V1/Aluno
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

        /// <summary>
        /// Método responsável por atualizar aluno 
        /// </summary>
        /// <param name="id"></param>
        /// /// <param name="model"></param>
        /// <returns></returns>
        // PUT: api/V1/Aluno/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, AlunoRegistrarDto model)
        {
            var aluno = _repository.GetAlunoById(id, false);
            if (aluno == null) return BadRequest("Aluno não encontrado");

            _mapper.Map(model, aluno);

            _repository.Update(aluno);

            if (_repository.SaveChanges())
            {
                return Created($"/api/aluno/{model.Id}", _mapper.Map<AlunoDto>(aluno));
            }

            return BadRequest("Aluno não atualizado");
        }

        /// <summary>
        /// Método responsável por atualizar aluno 
        /// </summary>
        /// <param name="id"></param>
        /// /// <param name="model"></param>
        /// <returns></returns>
        // PATCH: api/V1/Aluno/5
        [HttpPatch("{id}")]
        public IActionResult Patch(int id, AlunoRegistrarDto model)
        {
            var aluno = _repository.GetAlunoById(id, false);
            if (aluno == null) return BadRequest("Aluno não encontrado");

            _mapper.Map(model, aluno);

            _repository.Update(aluno);

            if (_repository.SaveChanges())
            {
                return Created($"/api/aluno/{model.Id}", _mapper.Map<AlunoDto>(aluno));
            }

            return BadRequest("Aluno não atualizado");
        }

        /// <summary>
        /// Método responsável por deletar aluno 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // DELETE: api/V1/Aluno/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var aluno = _repository.GetAlunoById(id, false);
            if (aluno == null) return BadRequest("Aluno não encontrado");

            _repository.Delete(aluno);

            if (_repository.SaveChanges())
            {
                return Ok("Aluno deletado");
            }

            return BadRequest("Aluno não foi deletado");
        }
    }
}
