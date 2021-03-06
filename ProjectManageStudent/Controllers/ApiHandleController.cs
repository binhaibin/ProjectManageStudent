﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectManageStudent.Models;

namespace ProjectManageStudent.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiHandleController : ControllerBase
    {
        private readonly ProjectManageStudentContext _context;

        public ApiHandleController(ProjectManageStudentContext context)
        {
            _context = context;
        }

        [HttpGet("Information/{Account}")]
        public async Task<IActionResult> Information()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var basicToken = Request.Headers["Authorization"].ToString();
            var token = basicToken.Replace("Basic ", "");
            var existToken = _context.Credential.SingleOrDefault(a => a.AccessToken == token);
            if (existToken != null)
            {
                if (existToken.Status == CredentialStatus.Active && existToken.ExpiredAt >= DateTime.Now)
                {
                    var Id = existToken.OwnerId;
                    var existAccount = _context.Account.SingleOrDefault(a => a.Id == Id);
                    if (existAccount != null)
                    {
                        Response.StatusCode = (int)HttpStatusCode.NotFound; 
                        return new JsonResult(existAccount);
                    }
                    Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    return new JsonResult("Forbidden");
                }
            }
            Response.StatusCode = (int)HttpStatusCode.Forbidden;
            return new JsonResult("Not Found");
        }

//TO DO
        [HttpGet("ListStudentInClass/{classroomId}")]
        public IEnumerable<Account> ListStudent(string classroom)
        {
            return _context.Account;
        }

        [HttpGet("Subject/{account}")]
        public IEnumerable<Subject> ListSubject(string Account)
        {
            return _context.Subject;
        }

        [HttpGet("Mark")]
        public IEnumerable<Mark> Mark(string Account)
        {
            return _context.Mark;
        }
    }
}