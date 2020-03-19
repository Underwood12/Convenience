﻿
using backend.fluentvalidation;
using backend.model.backend.api.AccountViewModels;
using backend.service.backend.api.Account;
using Backend.Api.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using System.Threading.Tasks;

namespace backend.api.Controllers
{
    [Route("api")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _loginService;

        public AccountController(IAccountService loginService)
        {
            _loginService = loginService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            var result = await _loginService.ValidateCredentials(model.UserName, model.Password);
            if (string.IsNullOrEmpty(result))
            {
                return this.BadRequestResult("错误的用户名或密码！");
            }
            return Ok(new LoginResult
            {
                UserName = model.UserName,
                Token = result
            });
        }

        [HttpPost("password")]
        [Authorize]
        public async Task<IActionResult> ChangePwdByOldPwd(ChangePwdViewModel viewmodel)
        {
            var result = await _loginService.ChangePassword(User.GetUserName(), viewmodel.OldPassword, viewmodel.NewPassword);
            if (!result)
            {
                return this.BadRequestResult("密码修改失败,请确认旧密码是否正确！");
            }            
            return Ok();
        }
    }
}