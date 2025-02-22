using System;
using System.Linq;
using System.Web.Http;
using BusinessEntities;
using Data;
using WebApi.Models.Users;
using System.Runtime.Serialization;

namespace WebApi.Controllers
{
    public class UserController : ApiController
    {
        private readonly IRepository<User> _userRepository;
        private readonly UserRepository _userRepositoryFiltered;

        public UserController(IRepository<User> userRepository)
        {
            _userRepository = userRepository;
            _userRepositoryFiltered = userRepository as UserRepository;
        }

        [HttpPost]
        [Route("api/users")]
        public IHttpActionResult Create([FromBody] CreateUserModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var user = new User(
                    Guid.NewGuid(),
                    model.Name,
                    model.Email,
                    model.MonthlySalary
                );

                _userRepository.Add(user);
                return Ok(new UserData(user));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPut]
        [Route("api/users/{id}")]
        public IHttpActionResult Update(Guid id, [FromBody] UpdateUserModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var existingUser = _userRepository.GetById(id);
                if (existingUser == null)
                    return NotFound();

                var updatedUser = new User(
                    existingUser.Id,
                    model.Name,
                    model.Email,
                    model.MonthlySalary
                );

                _userRepository.Update(updatedUser);
                return Ok(new UserData(updatedUser));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpDelete]
        [Route("api/users/{id}")]
        public IHttpActionResult Delete(Guid id)
        {
            try
            {
                var user = _userRepository.GetById(id);
                if (user == null)
                    return NotFound();

                _userRepository.Delete(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("api/users/{id}")]
        public IHttpActionResult Get(Guid id)
        {
            try
            {
                var user = _userRepository.GetById(id);
                if (user == null)
                    return NotFound();

                return Ok(new UserData(user));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("api/users")]
        public IHttpActionResult List(string nameFilter = null, string emailFilter = null, decimal? minSalary = null, decimal? maxSalary = null)
        {
            try
            {
                if (_userRepositoryFiltered != null)
                {
                    var users = _userRepositoryFiltered.GetFiltered(nameFilter, emailFilter, minSalary, maxSalary);
                    return Ok(users.Select(u => new UserData(u)));
                }
                else
                {
                    var users = _userRepository.GetAll()
                        .Where(u =>
                            (string.IsNullOrEmpty(nameFilter) || u.Name.Contains(nameFilter)) &&
                            (string.IsNullOrEmpty(emailFilter) || u.Email.Contains(emailFilter)) &&
                            (!minSalary.HasValue || u.MonthlySalary >= minSalary.Value) &&
                            (!maxSalary.HasValue || u.MonthlySalary <= maxSalary.Value)
                        )
                        .ToList();

                    return Ok(users.Select(u => new UserData(u)));
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }

    [DataContract]
    public class UserData
    {
        [DataMember]
        public string Id { get; set; }
        
        [DataMember]
        public string Name { get; set; }
        
        [DataMember]
        public string Email { get; set; }
        
        [DataMember]
        public decimal? MonthlySalary { get; set; }

        public UserData()
        {
        }

        public UserData(User user)
        {
            Id = user.Id.ToString();
            Name = user.Name;
            Email = user.Email;
            MonthlySalary = user.MonthlySalary;
        }
    }
}