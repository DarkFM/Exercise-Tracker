using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExerciseTracker.Domain.Requests;
using ExerciseTracker.Domain.Responses.Exercise;
using ExerciseTracker.Domain.Responses.User;
using ExerciseTracker.Domain.Services;
using ExerciseTracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ExerciseTracker.Controllers
{
    [ApiController]
    [Route("api/exercise")]
    public class AppController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<AppController> _logger;
        private readonly IExerciseService _exerciseService;

        public AppController(ILogger<AppController> logger, IUserService userService, IExerciseService exerciseService)
        {
            _logger = logger;
            _userService = userService;
            _exerciseService = exerciseService;
        }

        [HttpPost("new-user")]
        public async Task<ActionResult<UserInfoResponse>> AddNewUser([FromForm]NewUserRequest request)
        {
            var user = await _userService.GetUserByNameAsync(request.Username);
            if (user != null)
            {
                ModelState.AddModelError("", "Username already taken");
                return BadRequest(ModelState);
            }
            _logger.LogInformation("Adding new user: {userName}", request.Username);
            return await _userService.AddUserAsync(request);
        }

        [HttpPost("add")]
        public async Task<ExerciseResponse> AddNewExercise([FromForm]NewExerciseRequest request)
        {
            _logger.LogInformation("Adding a new Exercise for user: {userId}", request.UserId);
            return await _exerciseService.AddExerciseAsync(request);
        }

        [HttpGet("users")]
        public async Task<IEnumerable<UserInfoResponse>> GetAllUsers()
        {
            _logger.LogInformation("Getting list of all users registered in the app");
            return await _userService.GetUsersAsync();
        }

        [HttpGet("log")]
        public async Task<ActionResult<UserDetailsResponse>> GetUserLogs(Guid userId, DateTime from, DateTime to, int? limit)
        {
            _logger.LogInformation("Getting the user logs for {userId}", userId);
            var users = await _userService.GetUserLogsAsync(userId, limit, from, to);

            if (users == null)
            {
                ModelState.AddModelError("", "Error Getting the specified user. User might not exist");
                return BadRequest(ModelState);
            }

            return users;
        }

    }
}
